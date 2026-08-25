namespace AIKnowledgeApp {
    public class AskResponse {

        public string question { get; set; }
        public string answer { get; set; }
        public List<SourceInfo> sources { get; set; }
        public List<double> distances { get; set; }
    }

    public class SourceInfo {
        public string filename { get; set; }
        public int page { get; set; }
        public int chunk_index { get; set; }
    }
}
