namespace Coursework_CrystalSite_Final.Models
{
    public class PropertyTableModel
    {
        public string PropertyName { get; set; }

        public string ChemicalName { get; set; }

        public TableWithSyngonyModel TableWithSyngonyModel { get; set; }

        public TableWithoutSyngonyModel TableWithoutSyngonyModel { get; set; }
    }
}
