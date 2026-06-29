using System;
using UnityEngine;

[Serializable]
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
    public int productId;
    public string productName;
    public string category;
    public decimal price;
    public int stock;
    public DateTime createdAt;
    }
