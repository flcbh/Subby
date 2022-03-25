using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Subby.Workers.Models
{
    
    [XmlRoot(ElementName="jobs")]
    public class CvLibraryResponse {
        [XmlElement(ElementName="job")]
        public List<CvLibraryJob> Job { get; set; }
    }
    
    [XmlRoot(ElementName="job")]
    public class CvLibraryJob {
        [XmlElement(ElementName="jobref")]
        public string Jobref { get; set; }
        [XmlElement(ElementName="date")]
        public string Date { get; set; }
        [XmlElement(ElementName="title")]
        public string Title { get; set; }
        [XmlElement(ElementName="company")]
        public string Company { get; set; }
        [XmlElement(ElementName="email")]
        public string Email { get; set; }
        [XmlElement(ElementName="url")]
        public string Url { get; set; }
        [XmlElement(ElementName="salarymin")]
        public string Salarymin { get; set; }
        [XmlElement(ElementName="salarymax")]
        public string Salarymax { get; set; }
        [XmlElement(ElementName="benefits")]
        public string Benefits { get; set; }
        [XmlElement(ElementName="salary")]
        public string Salary { get; set; }
        [XmlElement(ElementName="jobtype")]
        public string Jobtype { get; set; }
        [XmlElement(ElementName="full_part")]
        public string Full_part { get; set; }
        [XmlElement(ElementName="salary_per")]
        public string Salary_per { get; set; }
        [XmlElement(ElementName="location")]
        public string Location { get; set; }
        [XmlElement(ElementName="city")]
        public string City { get; set; }
        [XmlElement(ElementName="county")]
        public string County { get; set; }
        [XmlElement(ElementName="country")]
        public string Country { get; set; }
        [XmlElement(ElementName="description")]
        public string Description { get; set; }
        [XmlElement(ElementName="category")]
        public string Category { get; set; }
        [XmlElement(ElementName="image")]
        public string Image { get; set; }
    }
}