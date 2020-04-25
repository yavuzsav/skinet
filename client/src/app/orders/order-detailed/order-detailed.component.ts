import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {BreadcrumbService} from 'xng-breadcrumb';
import {OrdersService} from '../orders.service';
import {IOrder} from '../../shared/models/order';

@Component({
  selector: 'app-order-detailed',
  templateUrl: './order-detailed.component.html',
  styleUrls: ['./order-detailed.component.scss']
})
export class OrderDetailedComponent implements OnInit {
  order: IOrder;

  constructor(private activatedRoute: ActivatedRoute, private breadcrumbService: BreadcrumbService,
              private ordersService: OrdersService) {
    this.breadcrumbService.set('@OrderDetailed', '');
  }

  ngOnInit(): void {
    this.getOrderDetail();
  }

  getOrderDetail() {
    this.ordersService.getOrderDetailed(+this.activatedRoute.snapshot.paramMap.get('id'))
      .subscribe((order: IOrder) => {
        this.order = order;
        this.breadcrumbService.set('@OrderDetailed', `Order# ${order.id} - ${order.orderStatus}`);
      }, error => {
        console.log(error);
      });
  }

}
