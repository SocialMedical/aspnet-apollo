using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sic.Web.Mvc.Report
{
    public class WebFormReportSettings
    {
        public WebFormReportSettings()
        {
        }

        public WebFormReportSettings(string reportPath)
        {
            this.ReportPath = reportPath;
        }

        private List<ReportDataSource> dataSources;
        private List<ReportParameter> parameters;

        public string ReportPath { get; set; }

        public List<ReportDataSource> DataSources
        {
            get
            {
                if (dataSources == null)
                    dataSources = new List<ReportDataSource>();
                return dataSources;
            }
        }

        public List<ReportParameter> Parameters
        {
            get
            {
                if (parameters == null)
                    parameters = new List<ReportParameter>();
                return parameters;
            }
        }

        public void AddDataSource(ReportDataSource reportDataSource)
        {
            this.DataSources.Add(reportDataSource);
        }

        public void AddDataSource(string name, object dataSourceValue)
        {
            object result = dataSourceValue;
            if(!(dataSourceValue is System.Data.DataTable || dataSourceValue is System.Collections.IEnumerable || dataSourceValue is System.Web.UI.IDataSource))
                result = new List<object>() { dataSourceValue };

            this.DataSources.Add(new ReportDataSource(name, result));
        }


        public void AddParameter(ReportParameter reportParameter)
        {
            this.Parameters.Add(reportParameter);
        }

        public void AddParameter(string name, string parameterValue)
        {
            AddParameter(new ReportParameter(name, parameterValue));            
        }
    }
}
