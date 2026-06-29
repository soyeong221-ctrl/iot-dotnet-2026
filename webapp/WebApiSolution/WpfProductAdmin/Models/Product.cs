using System;
using System.Collections.Generic;
using System.Text;

namespace WpfProductAdmin.Models
{
    public class Product
    {
        /*
         *  product_id INT NOT NULL AUTO_INCREMENT Primary Key,
            product_name VARCHAR(100) NOT NULL,
            category VARCHAR(50) NULL,
            price DECIMAL(10,0) NOT NULL,
            stock INT NOT NULL,
            created_at DATETIME
         */
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        // ? nullable
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}