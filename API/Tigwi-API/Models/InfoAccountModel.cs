﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Tigwi_API.Models
{
    [Serializable]
    [XmlType("List")]
    public class ListApi
    {
        public ListApi(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    [Serializable]
    public class Lists
    {
        public Lists()
        {
            List = new List<ListApi>();
            Size = 0;
        }
        public Lists(List<ListApi> list)
        {
            List = list;
            Size = list.Count();
        }

        [XmlAttribute]
        public int Size { get; set; }

        [XmlElement]
        public List<ListApi> List;
    }
}