namespace Coursework_CrystalSite_Final.Models
{
    public class ImageQueryModel
    {
        public string ChemicalFormula { get; set; }
        
        public string PropertyName { get; set; }

        public List<ImageModel> Images { get; set; }
    }
}
