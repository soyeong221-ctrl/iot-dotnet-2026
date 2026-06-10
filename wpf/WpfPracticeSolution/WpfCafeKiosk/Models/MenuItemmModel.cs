namespace WpfCafeKiosk.Models
{
    public class MenuItemModel
    {
        // menu_id, menu_name, price, image_path, category, is_sale DB 컬럼명
        // MenuId, MenuName, Price, ImagePath, Category, IsSale 클래스 속성명

        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public int Price { get; set; }
        public string ImagePath { get; set; }

        public string Category { get; set; }

    }
}
