#region License

/*
 * Copyright 2002-2007 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

#region Imports

using System;
using System.Collections;

using Spring.Transaction.Interceptor;

using Spring.Northwind.Dao;
using Spring.Northwind.Domain;

#endregion

namespace Spring.Northwind.Service
{
    public class FulfillmentService : IFulfillmentService
    {
        #region Fields
        
        private IProductDao productDao;

        private ICustomerDao customerDao;

        private IOrderDao orderDao;

        private IShippingService shippingService;

        #endregion

        #region Properties

        public IProductDao ProductDao
        {
            get { return productDao; }
            set { productDao = value; }
        }

        public IOrderDao OrderDao
        {
            get { return orderDao; }
            set { orderDao = value; }
        }

        public ICustomerDao CustomerDao
        {
            get { return customerDao; }
            set { customerDao = value; }
        }

        public IShippingService ShippingService
        {
            get { return shippingService; }
            set { shippingService = value; }
        }

        #endregion
        
        #region Methods



        [Transaction(ReadOnly=false)]
        public void ProcessCustomer(string customerId)
        {
            //Find all orders for customer
            Customer customer = CustomerDao.FindById(customerId);
                 
            foreach (Order order in customer.Orders)
            {
                //Validate Order
                Validate(order);
                
                //Ship with external shipping service
                ShippingService.ShipOrder(order);
                
                //Update shipping date
                order.ShippedDate = DateTime.Now;
                
                //Update shipment date
                OrderDao.SaveOrUpdate(order);
                
                //Other operations...Decrease product quantity... etc
            }             
            
        }

        private void Validate(Order order)
        {
            
            //TODO throw exception on error.

        }

        #endregion
    }
}