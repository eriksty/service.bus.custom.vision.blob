﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ExampleServiceBus.Model;
using ExampleServiceBus.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.EntityFrameworkCore;

namespace ExampleServiceBus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private DataContext db = new DataContext();

        [HttpPost]
        public async Task Post(Teste value)
        {
            await SendQueue.SendMessagesAsync(value);
            await SendQueue.Finish();
        }

        [HttpGet("azure")]
        public async Task<IActionResult> GetQueueAzure()
        {
            await GetQueue.RegisterOnMessageHandlerAndReceiveMessages();
            return Ok();
        }

        [HttpGet("findAll")]
        public async Task<IActionResult> FindAll()
        {
            try
            {
                var produtos = await db.Teste.ToListAsync();
                return Ok(produtos);
            }
            catch (Exception msg)
            {
                throw new Exception(msg.Message);
            }
        }

        [HttpPost("test")]
        public async Task<IActionResult> Post(List<IFormFile> img)
        {
            var requestStream = Request.HttpContext.Items;

            const string scue = "https://southcentralus.api.cognitive.microsoft.com";
            string trainingKey = "4f473807b7434dd5a1bdb45cb9104b38";

            CustomVisionTrainingClient trainingApi = new CustomVisionTrainingClient()
            {
                ApiKey = trainingKey,
                Endpoint = scue
            };

            // Find the object detection domain
            var domains = trainingApi.GetDomainsAsync();
            //var objDetectionDomain = domains.FirstOrDefault(d => d.Type == "ObjectDetection");
            var project = trainingApi.GetProject(Guid.Parse("b911d77a-ef25-47fd-86ed-87db4500ef7b"));


            const string southcentralus = "https://southcentralus.api.cognitive.microsoft.com";

            string predictionKey = "a58f3ca5856c491db0b73b87cb1118cf";

            CustomVisionPredictionClient endpoint = new CustomVisionPredictionClient()
            {
                ApiKey = predictionKey,
                Endpoint = southcentralus
            };


            //foreach (var item in img)
            //{
            //    var result = endpoint.PredictImage(project.Id, item.OpenReadStream());

            //    foreach (var c in result.Predictions)
            //    {
            //        //Console.WriteLine($"\t{c.TagName}: {c.Probability:P1} [ {c.BoundingBox.Left}, {c.BoundingBox.Top}, {c.BoundingBox.Width}, {c.BoundingBox.Height} ]");
            //        return Ok(c);
            //    }
            //}
            var result = endpoint.PredictImage(project.Id, img[0].OpenReadStream());

            //var a = Math.Round(result.Predictions[0].Probability, 5);
            //var b = Math.Round(result.Predictions[1].Probability, 5);

            //var c = result.Predictions[0];
            //var d = result.Predictions[1];

            return Ok(result.Predictions);
        }
    }


}

