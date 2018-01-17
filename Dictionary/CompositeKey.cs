namespace CourseProject.Dictionary
{
    public sealed class CompositeKey<TId, TName>
    {
        public TId Id { get; }
        public TName Name { get; }

        public CompositeKey(TId id, TName name)
        {
            Id = id;
            Name = name;
        }
    }
}
