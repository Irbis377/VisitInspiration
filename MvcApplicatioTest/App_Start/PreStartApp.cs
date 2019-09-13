using MvcApplicatioTest.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebActivatorEx;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(PreStartApp), "Start")]
namespace MvcApplicatioTest.App_Start
{
    public class PreStartApp
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Метод запускается один раз перед стартом приложения        
        /// </summary>
        public static void Start()
        {
            logger.Info("Application PreStart");
        }
    }
}