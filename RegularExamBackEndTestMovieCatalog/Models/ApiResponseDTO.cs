using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RegularExamBackEndTestMovieCatalog.Models
{
    public class AirResponseDTO
    {
        [JsonPropertyName("msg")]

        public string Msg { get; set; }

        [JsonPropertyName("movie")]

        public MovieDTO Movie { get; set; } = new MovieDTO();

    }
}
