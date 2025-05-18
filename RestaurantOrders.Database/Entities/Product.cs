using RestaurantOrders.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantOrders.Database.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Category ProdCategory { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        public decimal Price { get; set; }
        public List<Allergen>? Allergens { get; set; }

        // TODO 3: MENIURILE TREBUIE SA AIBA SI ELE O CATEGORIE (ex: un meniu este considerat o grupare de mai multe produse)
        // TODO 4: Pretul meniului se va calcula in functie de pretul preparatelor care il alcatuiesc cu o reducere x% (reducerea se va lua din fisierul de config)
        // TODO 5: pentru fiecare produs din meniu vor fi afisate cantitatile pentru fiecare produs in parte, care nu trb neaparat sa corespunda cu cantitatile produselor efective
    }
}
