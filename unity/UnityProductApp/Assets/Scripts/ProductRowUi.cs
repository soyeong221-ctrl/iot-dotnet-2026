using System;
using TMPro;
using UnityEngine;

public class ProductRowUi : MonoBehaviour
{
    [SerializeField] private TMP_Text txtProductId;
    [SerializeField] private TMP_Text txtProductName;
    [SerializeField] private TMP_Text txtCategory;
    [SerializeField] private TMP_Text txtPrice;
    [SerializeField] private TMP_Text txtStock;
    [SerializeField] private TMP_Text txtCreatedAt;

    public void SetData(Product product)
    {
        txtProductId.text = product.productId.ToString();
        txtProductName.text = product.productName;
        txtCategory.text = product.category;
        txtPrice.text = $"{product.price:N0}원";
        txtStock.text = $"{product.stock:N0}";
        txtCreatedAt.text = product.createdAt.ToString("yyyy-MM-dd");
    }

}
