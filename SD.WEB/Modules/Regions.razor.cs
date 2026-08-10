namespace SD.WEB.Modules
{
    public partial class Regions
    {
        public IEnumerable<EnumFieldObject<Country>> RegionsList { get; set; } = [];

        private List<string?> Options { get; set; } = [];
        private string? filter;

        protected override async Task LoadStaticDataAsync()
        {
            filter = Culture;

            RegionsList = EnumHelper.GetList<Country>();

            Options.Add("en");
            Options.Add("pt");
            Options.Add("es");
            Options.Add("fr");
            Options.Add("it");
            Options.Add("de");
        }

        public IEnumerable<EnumFieldObject<Country>> GetRegions()
        {
            if (string.Equals(filter, "en", StringComparison.OrdinalIgnoreCase))
            {
                return RegionsList;
            }

            if (string.Equals(filter, "pt", StringComparison.OrdinalIgnoreCase))
            {
                var portuguese = new HashSet<Country> { Country.BR, Country.PT };

                return RegionsList.Where(p => portuguese.Contains(p.Value));
            }

            if (string.Equals(filter, "es", StringComparison.OrdinalIgnoreCase))
            {
                var spanish = new HashSet<Country> { Country.ES, Country.MX, Country.AR, Country.CO };

                return RegionsList.Where(p => spanish.Contains(p.Value));
            }

            if (string.Equals(filter, "fr", StringComparison.OrdinalIgnoreCase))
            {
                var french = new HashSet<Country> { Country.FR, Country.CA, Country.BE, Country.CH };

                return RegionsList.Where(p => french.Contains(p.Value));
            }

            if (string.Equals(filter, "it", StringComparison.OrdinalIgnoreCase))
            {
                var italian = new HashSet<Country> { Country.IT, Country.CH, };

                return RegionsList.Where(p => italian.Contains(p.Value));
            }

            if (string.Equals(filter, "de", StringComparison.OrdinalIgnoreCase))
            {
                var german = new HashSet<Country> { Country.DE, Country.AT, Country.CH, };

                return RegionsList.Where(p => german.Contains(p.Value));
            }

            return [];
        }
    }
}