using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TIApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TIApi.Controllers
{
    [Route("operation")]
    public class OperationController: Controller
    {
        private IHostingEnvironment hostingEnvironment;

        private readonly string path;

        private XmlDocument xmldoc;

        private void LoadXmlDocument()
        {
            var fs = new FileStream(this.path, FileMode.Open, FileAccess.Read);
            this.xmldoc = new XmlDocument();
            this.xmldoc.Load(fs);
            fs.Close();
        }

        private void WriteXmlDocument()
        {
            var fs = new FileStream(this.path, FileMode.Create, FileAccess.Write);
            fs.Flush();
            this.xmldoc.Save(fs);
            fs.Close();
        }

        public OperationController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.path = this.hostingEnvironment.WebRootPath + "\\uploads\\" + "Projekt.tia";
        }

        [HttpGet]
        [Route("list")]
        public string List()
        {
            this.LoadXmlDocument();
            var xmlnode = this.xmldoc.GetElementsByTagName("node");
            var list = new List<JsonDevice>();
            for (var i = 0; i <= xmlnode.Count - 1; i++)
            {
                var currentNode = xmlnode.Item(i);
                if (currentNode.Attributes["Type"].Value == "Device")
                {
                    var jsonDevice = new JsonDevice();

                    foreach (XmlNode childNode in currentNode.SelectNodes("properties"))
                    {
                        foreach (XmlNode child in childNode.SelectNodes("property"))
                        {
                            if (child["key"].InnerText == "Name")
                            {
                                jsonDevice.DeviceName = (child["value"].InnerText);
                            }
                            if (child["key"].InnerText == "LastWriteTime")
                            {
                                jsonDevice.LastWriteTime = child["value"].InnerText;
                            }
                            if (child["key"].InnerText == "Id")
                            {
                                jsonDevice.Id = child["value"].InnerText;
                            }
                        }
                    }
                    list.Add(jsonDevice);
                }
            }
            var json = JsonConvert.SerializeObject(list);
            return json;
        }

        [HttpGet]
        [Route("{id}")]
        public string GetDevice(string id)
        {
            this.LoadXmlDocument();
            var xmlnode = this.xmldoc.GetElementsByTagName("node");
            for (var i = 0; i <= xmlnode.Count - 1; i++)
            {
                var currentNode = xmlnode.Item(i);
                if (currentNode.Attributes["Type"].Value == "Device")
                {
                    foreach (XmlNode childNode in currentNode.SelectNodes("properties"))
                    {
                        foreach (XmlNode child in childNode.SelectNodes("property"))
                        {
                            if (child["key"].InnerText == "Id")
                            {
                                if (child["value"].InnerText == id)
                                {
                                    return currentNode.OuterXml;
                                }
                            }
                        }
                    }
                }
            }

            return "No such device";
        }

        [HttpGet]
        [Route("add/{id}")]
        public string Add(string id)
        {
            this.LoadXmlDocument();

            var xmlnode = this.xmldoc.GetElementsByTagName("node");

            var parentNode = xmlnode.Item(0).ParentNode;
            var nodeToAdd = this.xmldoc.CreateElement("node");
            nodeToAdd.SetAttribute("Type", "Device");
            //just hardcoded 
            nodeToAdd.InnerXml = ($"<properties><property><key>Id</key><value>{id}</value></property></properties>");
            parentNode.AppendChild(nodeToAdd);

            this.WriteXmlDocument();

            return $"Device with Id: {id} added";
        }

        [HttpGet]
        [Route("delete/{id}")]
        public string Delete(string id)
        {
            this.LoadXmlDocument();
            var xmlnode = this.xmldoc.GetElementsByTagName("node");

            for (var i = 0; i <= xmlnode.Count - 1; i++)
            {
                var currentNode = xmlnode.Item(i);
                if (currentNode.Attributes["Type"].Value == "Device")
                {
                    foreach (XmlNode childNode in currentNode.SelectNodes("properties"))
                    {
                        foreach (XmlNode child in childNode.SelectNodes("property"))
                        {
                            if (child["key"].InnerText == "Id")
                            {
                                if (child["value"].InnerText == id)
                                {
                                    currentNode.ParentNode.RemoveChild(currentNode);
                                    this.WriteXmlDocument();

                                }
                            }
                        }
                    }
                }
            }

            return $"Device with Id: {id} deleted";
        }
    }
}
