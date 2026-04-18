using System.Net;
using RegularExamBackEndTestMovieCatalog.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace RegularExamBackEndTestMovieCatalog
{
    [TestFixture]
    public class Tests
    {
        private RestClient client;

        private const string BaseUrl = "http://144.91.123.158:5000";

        private const string Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKd3RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiI1OTVkMDA2OS0yNTMyLTQ3NmItOTM0Ny0wNjJjODYwN2Y3ZGQiLCJpYXQiOiIwNC8xOC8yMDI2IDA2OjQ0OjU2IiwiVXNlcklkIjoiOWRjY2FmNzgtMzMwOC00ZTY3LTYyNzgtMDhkZTc2OTcxYWI5IiwiRW1haWwiOiJrYW1teTExMjJAZ21haWwuY29tIiwiVXNlck5hbWUiOiJrYW1teTExMjIiLCJleHAiOjE3NzY1MTYyOTYsImlzcyI6Ik1vdmllQ2F0YWxvZ19BcHBfU29mdFVuaSIsImF1ZCI6Ik1vdmllQ2F0YWxvZ19XZWJBUElfU29mdFVuaSJ9.IGQH2a2W_wye7yM-XtMEpp9AovGXsSp_Opr65m2mQ1I";

        private static string createdMovieId;

        private const string LoginEmail = "kammy1122@gmail.com";
        private const string LoginPassword = "112233";



        [OneTimeSetUp]
        public void Setup()
        {
            var options = new RestClientOptions(BaseUrl)
            {
                Authenticator = new JwtAuthenticator(Token)
            };

            client = new RestClient(options);
        }

        [Test, Order(1)]
        public void CreateMovie_WithRequiredFields_ShouldCreateSuccessfully()
        {
            var request = new RestRequest("/api/Movie/Create", Method.Post);

            request.AddJsonBody(new
            {
                title = "Test Movie",
                description = "Test Description"
            });

            var response = client.Execute<AirResponseDTO>(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data, Is.Not.Null);
            Assert.That(response.Data.Movie, Is.Not.Null);
            Assert.That(response.Data.Movie.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(response.Data.Msg, Is.EqualTo("Movie created successfully!"));

            createdMovieId = response.Data.Movie.Id;
        }

        [Test, Order(2)]
        public void EditMovie_ShouldEditSuccessfully()
        {
            var request = new RestRequest("/api/Movie/Edit", Method.Put);

            request.AddQueryParameter("movieId", createdMovieId);

            request.AddJsonBody(new
            {
                title = "Edited Movie",
                description = "Edited Description"
            });

            var response = client.Execute<AirResponseDTO>(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data, Is.Not.Null);
            Assert.That(response.Data.Msg, Is.EqualTo("Movie edited successfully!"));
        }

        [Test, Order(3)]
        public void GetAllMovies_ShouldReturnNonEmptyList()
        {
            var request = new RestRequest("/api/Catalog/All", Method.Get);

            var response = client.Execute<List<MovieDTO>>(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data, Is.Not.Null);
            Assert.That(response.Data.Count, Is.GreaterThan(0));
        }

        [Test, Order(4)]
        public void DeleteMovie_ShouldDeleteSuccessfully()
        {
            var request = new RestRequest("/api/Movie/Delete", Method.Delete);

            request.AddQueryParameter("movieId", createdMovieId);

            var response = client.Execute<AirResponseDTO>(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data, Is.Not.Null);
            Assert.That(response.Data.Msg, Is.EqualTo("Movie deleted successfully!"));
        }

        [Test, Order(5)]
        public void CreateMovie_WithoutRequiredFields_ShouldReturnBadRequest()
        {
            var request = new RestRequest("/api/Movie/Create", Method.Post);

            request.AddJsonBody(new
            {
                title = "",
                description = ""
            });

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test, Order(6)]
        public void EditNonExistingMovie_ShouldReturnBadRequest()
        {
            var request = new RestRequest("/api/Movie/Edit", Method.Put);

            request.AddQueryParameter("movieId", "123456");

            request.AddJsonBody(new
            {
                title = "Fake Movie",
                description = "Fake Description"
            });

            var response = client.Execute<AirResponseDTO>(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Data, Is.Not.Null);
            Assert.That(response.Data.Msg,
                Is.EqualTo("Unable to edit the movie! Check the movieId parameter or user verification!"));
        }

        [Test, Order(7)]
        public void DeleteNonExistingMovie_ShouldReturnBadRequest()
        {
            var request = new RestRequest("/api/Movie/Delete", Method.Delete);

            request.AddQueryParameter("movieId", "123456");

            var response = client.Execute<AirResponseDTO>(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Data, Is.Not.Null);
            Assert.That(response.Data.Msg,
                Is.EqualTo("Unable to delete the movie! Check the movieId parameter or user verification!"));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            client?.Dispose();
        }
    }
}