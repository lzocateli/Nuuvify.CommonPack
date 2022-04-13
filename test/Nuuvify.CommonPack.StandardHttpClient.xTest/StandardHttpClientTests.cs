﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Nuuvify.CommonPack.Security.Abstraction;
using Nuuvify.CommonPack.StandardHttpClient.Results;
using Nuuvify.CommonPack.StandardHttpClient.xTest.Configs;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Nuuvify.CommonPack.StandardHttpClient.xTest
{

    public class StandardHttpClientTests
    {

        private readonly Mock<IHttpClientFactory> mockFactory;
        private readonly IConfiguration Config;

        public StandardHttpClientTests()
        {
            mockFactory = new Mock<IHttpClientFactory>();

            Config = AppSettingsConfig.GetConfig();
        }


        [Fact, Order(1)]
        public async Task GetEmApiMockDeveRetornarMensagemOK()
        {
            var fakeClassReturn = new FakeClasseRetorno
            {
                Codigo = 123456,
                DataCadastro = DateTime.Now,
                Descricao = "Isso é um teste"
            };

            var jsonConverted = JsonSerializer.Serialize(fakeClassReturn);

            var resultDefault = new HttpStandardReturn
            {
                ReturnCode = "200",
                ReturnMessage = jsonConverted,
                Success = true
            };

            var clientHandlerStub = new DelegatingHandlerStub(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonConverted, Encoding.UTF8, "application/json")
            });

            var client = new HttpClient(clientHandlerStub, true)
            {
                BaseAddress = new Uri("https://meuteste/")
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");


            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var url = "api/externafake";

            var standardClient = new StandardHttpClient(mockFactory.Object, new NullLogger<StandardHttpClient>());
            standardClient.CreateClient();

            var result = await standardClient
                .WithQueryString("codigo", fakeClassReturn.Codigo)
                .Get(url);

            var urlReturn = "https://meuteste/api/externafake?codigo=123456";

            Assert.Equal(resultDefault.ReturnCode, result.ReturnCode);
            Assert.Equal(resultDefault.ReturnMessage, result.ReturnMessage);
            Assert.Equal(resultDefault.Success, result.Success);
            Assert.Equal(urlReturn, standardClient.FullUrl.ToString());

        }

        [LocalTestFact, Order(2)]
        public void PostApiRealDeveRetornarMensagemComSucesso()
        {
            var config = AppSettingsConfig.GetConfig();
            var tokenFactory = new TokenFactory(config);
            var tokenValido = tokenFactory.ObtemTokenValido(
                loginId: config.GetSection("ApisCredentials:Username")?.Value,
                password: config.GetSection("ApisCredentials:Password")?.Value
            ).Result;

            var notification = tokenFactory?.Notifications.LastOrDefault();
            var expected = false;
            var actual = string.IsNullOrWhiteSpace(tokenValido);

            Assert.True(string.IsNullOrWhiteSpace(notification?.Message), notification?.Message);
            Assert.Equal(expected, actual);

        }

        [Fact, Order(3)]
        public void DeleteApiRealDeveRetornarMensagemComSucesso()
        {
            var resultDefault = new HttpStandardReturn
            {
                ReturnCode = "200",
                ReturnMessage = "Excluido com sucesso",
                Success = true
            };

            var jsonConverted = JsonSerializer.Serialize(resultDefault);

            var clientHandlerStub = new DelegatingHandlerStub(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonConverted, Encoding.UTF8, "application/json")
            });

            var client = new HttpClient(clientHandlerStub, true)
            {
                BaseAddress = new Uri("https://meuteste/")
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");


            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var url = "api/cliente";

            var standardClient = new StandardHttpClient(mockFactory.Object, new NullLogger<StandardHttpClient>());
            standardClient.CreateClient();
            standardClient.ResetStandardHttpClient();

            var result = standardClient
                .WithHeader("Accept-Language", "pt-BR")
                .WithCurrelationHeader("zxzxzxzxzxzxzxzxzxz")
                .WithAuthorization("bearer", "xyz")
                .Delete(url).Result;


            Assert.Equal(resultDefault.ReturnCode, result.ReturnCode);

        }

        [Fact]
        public void RecebeLoginDiferente_DeveSubstituirDados()
        {
            var usuarioAtual = "FulanoDeTal";
            var created = DateTimeOffset.Now;
            var expires = DateTimeOffset.Now.AddHours(8);
            var password = "XyZ";
            var token = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";


            var _credentialToken = new CredentialToken
            {
                LoginId = usuarioAtual,
                Created = created,
                Expires = expires,
                Password = password,
                Token = token,
            };


            _credentialToken.LoginId = "Giropopis";



            Assert.NotEqual(_credentialToken.LoginId, usuarioAtual);
            Assert.NotEqual(_credentialToken.Token, token);


        }


        [Fact]
        public void RecebeLoginIgual_NaoDeveSubstituirDados()
        {
            var usuarioAtual = "FulanoDeTal";
            var created = DateTimeOffset.Now;
            var expires = DateTimeOffset.Now.AddHours(8);
            var password = "XyZ";
            var token = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

            var _credentialToken = new CredentialToken
            {
                LoginId = usuarioAtual,
                Created = created,
                Expires = expires,
                Password = password,
                Token = token,
            };

            _credentialToken.LoginId = "fulanodetal";



            Assert.Equal(_credentialToken.LoginId, usuarioAtual);
            Assert.Equal(_credentialToken.Token, token);


        }

        [Fact]
        public void TokenEstaValido()
        {
            var usuarioAtual = "FulanoDeTal";
            var created = DateTimeOffset.Now;
            var expires = DateTimeOffset.Now.AddHours(8);
            var password = "XyZ";
            var token = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

            var _credentialToken = new CredentialToken
            {
                LoginId = usuarioAtual,
                Created = created,
                Expires = expires,
                Password = password,
                Token = token,
            };


            Assert.True(_credentialToken.IsValidToken());


        }

        [Fact]
        public void TokenEstaInvalido()
        {
            var usuarioAtual = "FulanoDeTal";
            var created = DateTimeOffset.Now.AddMinutes(-7);
            var expires = DateTimeOffset.Now;
            var password = "XyZ";
            var token = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

            var _credentialToken = new CredentialToken
            {
                LoginId = usuarioAtual,
                Created = created,
                Expires = expires,
                Password = password,
                Token = token,
            };




            Assert.False(_credentialToken.IsValidToken());


        }


    }


}
