namespace com.SML.BIGTRONS.ViewModels
{
    public abstract class FieldVM
    {
        protected string _map;
        protected string _mapAlias;

        public string Name { get { return this.GetType().Name.Substring(1); } }
        public string Map { get { return _map; } }
        public string MapAlias { get { return _mapAlias; } }
        public string Value { get; set; }

        //public FieldVM(string map, string mapAlias)
        //{
        //    _map = map;
        //    _mapAlias = mapAlias;
        //}
    }
}