using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }

        //[Cached()]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
            [FromQuery] ProductSpecParams productParams)
        {
            var spec = new ProductsWithTypesAndBrandsAndPhotosSpecification(productParams);

            var countSpec = new ProductWithFiltersForCountSpecification(productParams);

            var totalItems = await _unitOfWork.Repository<Product>().CountAsync(countSpec);

            var products = await _unitOfWork.Repository<Product>().ListAsync(spec);

            var data = _mapper.Map<List<Product>, List<ProductToReturnDto>>(products.ToList());

            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItems,
                data));
        }

        //[Cached()]
        [HttpGet("{id}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsAndPhotosSpecification(id);

            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);

            if (product == null) return NotFound(new ApiResponse(404));

            return _mapper.Map<Product, ProductToReturnDto>(product);
        }

        //[Cached()]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _unitOfWork.Repository<ProductBrand>().ListAllAsync());
        }

        //[Cached()]
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _unitOfWork.Repository<ProductType>().ListAllAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateProduct(ProductCreateDto productCreateDto)
        {
            var productType = await _unitOfWork.Repository<ProductType>().GetByIdAsync(productCreateDto.ProductTypeId);
            if (productType == null) return BadRequest(new ApiResponse(404, "Product type not found"));

            var productBrand =
                await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(productCreateDto.ProductBrandId);
            if (productBrand == null) return BadRequest(new ApiResponse(404, "Product brand not found"));

            var product = _mapper.Map<ProductCreateDto, Product>(productCreateDto);

            _unitOfWork.Repository<Product>().Add(product);

            var result = await _unitOfWork.Complete();

            if (result <= 0) return BadRequest(new ApiResponse(400, "Problem creating product"));

            product.ProductBrand = productBrand;
            product.ProductType = productType;
            var productToReturn = _mapper.Map<Product, ProductToReturnDto>(product);

            return CreatedAtRoute("GetProduct", new {id = product.Id}, productToReturn);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductToReturnDto>> UpdateProduct(int id, ProductCreateDto productCreateDto)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null) return BadRequest(new ApiResponse(404, "Product not found"));

            var productType = await _unitOfWork.Repository<ProductType>().GetByIdAsync(productCreateDto.ProductTypeId);
            if (productType == null) return BadRequest(new ApiResponse(404, "Product type not found"));

            var productBrand =
                await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(productCreateDto.ProductBrandId);
            if (productBrand == null) return BadRequest(new ApiResponse(404, "Product brand not found"));

            _mapper.Map(productCreateDto, product);

            _unitOfWork.Repository<Product>().Update(product);

            var result = await _unitOfWork.Complete();

            if (result <= 0) return BadRequest(new ApiResponse(400, "Problem updating product"));

            product.ProductBrand = productBrand;
            product.ProductType = productType;
            return _mapper.Map<Product, ProductToReturnDto>(product);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsAndPhotosSpecification(id);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);
            if (product == null) return BadRequest(new ApiResponse(404, "Product not found"));
            
            foreach (var photo in product.Photos)
            {
                if (photo.PublicId != null)
                    _photoService.DeletePhoto(photo.PublicId);
            }

            _unitOfWork.Repository<Product>().Delete(product);

            var result = await _unitOfWork.Complete();
            if (result <= 0) return BadRequest(new ApiResponse(400, "Problem deleting product"));
            return Ok();
        }

        [HttpPut("{productId}/photo")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductToReturnDto>> AddProductPhoto(int productId,
            [FromForm] ProductPhotoForCreationDto productPhotoForCreationDto)
        {
            var spec = new ProductsWithTypesAndBrandsAndPhotosSpecification(productId);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);

            if (product == null) return BadRequest(new ApiResponse(404, "Product not found"));

            var uploadResult = _photoService.AddPhoto(productPhotoForCreationDto.Photo);

            var photo = new Photo
            {
                ProductId = productId,
                Url = uploadResult.Url,
                PublicId = uploadResult.PublicId,
            };

            if (!product.Photos.Any(x => x.IsMain))
                photo.IsMain = true;


            // product.Photos.Add(photo);
            _unitOfWork.Repository<Photo>().Add(photo);

            var result = await _unitOfWork.Complete();

            if (result <= 0) return BadRequest(new ApiResponse(400, "Problem adding photo product"));

            return _mapper.Map<Product, ProductToReturnDto>(product);
        }

        [HttpDelete("{productId}/photo/{photoId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProductPhoto(int productId, int photoId)
        {
            var spec = new ProductsWithTypesAndBrandsAndPhotosSpecification(productId);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);
            var photo = product.Photos.SingleOrDefault(x => x.Id == photoId);

            if (photo == null)
                return BadRequest(new ApiResponse(404, "Product photo not found"));

            if (photo.IsMain)
                return BadRequest(new ApiResponse(400, "You can not delete the main photo"));

            _photoService.DeletePhoto(photo.PublicId);

            product.Photos.Remove(photo);

            var result = await _unitOfWork.Complete();

            if (result <= 0) return BadRequest(new ApiResponse(400, "Problem deleting photo product"));

            return Ok();
        }

        [HttpPost("{productId}/photo/{photoId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductToReturnDto>> SetMainPhoto(int productId, int photoId)
        {
            var spec = new ProductsWithTypesAndBrandsAndPhotosSpecification(productId);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);

            if (product.Photos.All(x => x.Id != photoId))
                return BadRequest(new ApiResponse(404, "Product photo not found"));

            var photo = product.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = product.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMainPhoto != null)
                currentMainPhoto.IsMain = false;

            photo.IsMain = true;

            _unitOfWork.Repository<Product>().Update(product);

            var result = await _unitOfWork.Complete();
            
            if (result <= 0) return BadRequest(new ApiResponse(400, "Problem adding photo product"));

            return _mapper.Map<Product, ProductToReturnDto>(product);
        }
    }
}