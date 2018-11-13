﻿using Eols.EPiGraphQL.Api.Models;
using EPiServer.ServiceLocation;
using GraphQL;
using GraphQL.Http;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Validation.Complexity;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Eols.EPiGraphQL.Api.Controllers
{
    [RoutePrefix("Api/GraphQL")]
    public class GraphQLController : ApiController
    {
        private readonly ISchema _schema;
        private readonly IDocumentExecuter _executer;
        private readonly IDocumentWriter _writer;

        public GraphQLController(
            IDocumentExecuter executer,
            IDocumentWriter writer,
            ISchema schema)
        {
            _executer = executer;
            _writer = writer;
            _schema = schema;
        }
        public GraphQLController() : this(
            ServiceLocator.Current.GetInstance<IDocumentExecuter>(),
            ServiceLocator.Current.GetInstance<IDocumentWriter>(),
            ServiceLocator.Current.GetInstance<ISchema>())
        {
        }
        
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> PostAsync(HttpRequestMessage request, GraphQLQuery query)
        {
            try
            {
                var inputs = query.Variables.ToInputs();
                var queryToExecute = query.Query;

                //if (!string.IsNullOrWhiteSpace(query.NamedQuery))
                //{
                //    queryToExecute = _namedQueries[query.NamedQuery];
                //}

                var result = await _executer.ExecuteAsync(_ =>
                {
                    _.Schema = _schema;
                    _.Query = queryToExecute;
                    _.OperationName = query.OperationName;
                    _.Inputs = inputs;

                    _.ComplexityConfiguration = new ComplexityConfiguration { MaxDepth = 15 };
                    _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();

                }).ConfigureAwait(false);

                result.ExposeExceptions = true;
                
                var httpResult = result.Errors?.Count > 0
                    ? HttpStatusCode.BadRequest
                    : HttpStatusCode.OK;

                var json = _writer.Write(result);

                var response = request.CreateResponse(httpResult);
                response.Content = new StringContent(json, Encoding.UTF8, "application/json");

                return response;
            }
            catch (Exception e)
            {
                return Task.FromResult<HttpResponseMessage>(
                    request.CreateErrorResponse(HttpStatusCode.BadRequest, e)).Result;
            }

        }        
    }
}