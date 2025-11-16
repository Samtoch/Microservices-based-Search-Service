namespace SemanticSearchService.Model
{
    public class EmbeddingRequest
    {
        public string CollectionName { get; set; }
        public List<string> Entries { get; set; }
    }
}
