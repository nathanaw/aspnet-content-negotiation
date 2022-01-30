namespace CodecExample.Data.Sqlite
{
    /// <summary>
    /// A class for use by dapper to read and write from the database.
    /// The structure of this class matches the columns in the database.
    /// 
    /// This subclass adds the "promoted" field "TempCelcius", which is
    /// used in some database queries.
    /// </summary>
    internal class WeatherForecastContentRow : EncodedContentRow
    {

        /// <summary>
        /// A "promoted column" for use in queries.
        /// </summary>
        /// <remarks>
        /// This is a copy of the value from the content. 
        /// </remarks>
        public int TempCelcius { get; set; }

    }

}
