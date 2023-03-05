namespace Headlines.DTO.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ObjectStorageNameAttribute : Attribute
    {
        public ObjectStorageNameAttribute(string name) 
        {
            Name = name;
        }

        public string Name { get; init; }
    }
}