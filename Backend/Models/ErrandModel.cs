namespace Backend.Models
{
    /// <summary>
    /// Single errand
    /// </summary>
    public class ErrandModel
    {
        /// <summary>
        /// Unique identifier for errand
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Free form description for errand
        /// </summary>
        public string Description { get; set; }
    }
}
