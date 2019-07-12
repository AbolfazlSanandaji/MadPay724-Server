﻿using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Presentation;
using MadPay724.Test.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MadPay724.Test.ControllersTests
{
    public class UsersControllerTests : IClassFixture<TestClientProvider<Startup>>
    {
        private HttpClient _client;
        private readonly string _UnToken;
        private readonly string _AToken;
        public UsersControllerTests(TestClientProvider<Startup> testClientProvider)
        {
            _client = testClientProvider.Client;
            _UnToken = "";
            //0d47394e-672f-4db7-898c-bfd8f32e2af7
            //haysmathis@barkarama.com
            //123789
            _AToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwZDQ3Mzk0ZS02NzJmLTRkYjctODk4Yy1iZmQ4ZjMyZTJhZjciLCJ1bmlxdWVfbmFtZSI6ImhheXNtYXRoaXNAYmFya2FyYW1hLmNvbSIsIm5iZiI6MTU2Mjg0NTM2NCwiZXhwIjoxNTYyOTMxNzY0LCJpYXQiOjE1NjI4NDUzNjR9.44SJQ97Zi_5lbNlGtp92xsjb6T0SrCWk2X8uCSgtCHN7BdbtsPJjX8T2GtcxlQ3H8x-JCaCJ9tBaSV_VhA7M-Q";
        }
        #region GetUserTests
        [Fact]
        public async Task GetUser_CantGetAnOtherUser()
        {
            // Arrange
            string anOtherUserId = "c5ba73d4-d9d8-4e2d-9fe3-b328b8f7f84b";
            var request = "/site/admin/Users/" + anOtherUserId;

            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _AToken);

            //Act
            var response = await _client.GetAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        }
        [Fact]
        public async Task GetUser_CanGetUserHimself()
        {
            // Arrange
            string userHimSelfId = "0d47394e-672f-4db7-898c-bfd8f32e2af7";
            var request = "/site/admin/Users/" + userHimSelfId;
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _AToken);

            //Act
            var response = await _client.GetAsync(request);

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
        #endregion

        #region UpdateUserTests
        [Fact]
        public async Task UpdateUser_CantUpdateAnOtherUser()
        {
            // Arrange
            string anOtherUserId = "c5ba73d4-d9d8-4e2d-9fe3-b328b8f7f84b";
            var request = new
            {
                Url = "/site/admin/Users/" + anOtherUserId,
                Body = new
                {
                    Name = "علی حسینی",
                    PhoneNumber = "string",
                    Address = "string",
                    Gender = true,
                    City = "string"
                }
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _AToken);

            //Act
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task UpdateUser_CanUpdateUserHimself()
        {
            // Arrange
            string userHimselfId = "0d47394e-672f-4db7-898c-bfd8f32e2af7";
            var request = new
            {
                Url = "/site/admin/Users/" + userHimselfId,
                Body = new
                {
                    Name = "علی حسینی",
                    PhoneNumber = "string",
                    Address = "string",
                    Gender = true,
                    City = "string"
                }
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _AToken);

            //Act
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_ModelStateError()
        {
            // Arrange
            string userHimselfId = "c5ba73d4-d9d8-4e2d-9fe3-b328b8f7f84b";
            var request = new
            {
                Url = "/site/admin/Users/" + userHimselfId,
                Body = new UserForUpdateDto
                {
                    Name = string.Empty,
                    PhoneNumber = string.Empty,
                    Address = string.Empty,
                    City = "لورم ایپسوم متن ساختگی با تولید سادگلورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی درلورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی درلورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی دری نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است چاپگرها و متون بلکه روزنامه مجله در ستون و سطر آنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد کتابهای زیادی در."
                }
            };
            _client.DefaultRequestHeaders.Authorization
           = new AuthenticationHeaderValue("Bearer", _AToken);

            var controller = new ModelStateControllerTests();
            

            //Act
            var response = await _client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();

            controller.ValidateModelState(request.Body);
            var modelState = controller.ModelState;

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            Assert.False(modelState.IsValid);
            Assert.Equal(4, modelState.Keys.Count());
            Assert.True(modelState.Keys.Contains("Name") && modelState.Keys.Contains("PhoneNumber")
                && modelState.Keys.Contains("Address") && modelState.Keys.Contains("City"));

        }
        #endregion

        #region ChangeUserPasswordTests

        #endregion
    }
}