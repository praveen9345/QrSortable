namespace QrSortable.Components.CoreFeatures.DataManagement.Models
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     The base model of all the database entries stored in the database.
    /// </summary>
    public class DatabaseEntry
    {
        /// <summary>
        ///     Gets the ID of the database entry which is set as the primary key for all the database entries.
        /// </summary>
        [JsonIgnore]
        [Key]
        public int ID { get; private set; }
    }
}
