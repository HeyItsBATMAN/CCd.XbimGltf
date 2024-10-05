﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Configuration;
using Xbim.Common.Geometry;
using Xbim.GLTF;
using Xbim.Ifc;
using Xbim.ModelGeometry.Scene;

namespace MainTests
{
    [TestClass]
    public class ConversionTests
    {
        [TestMethod]
        [DeploymentItem(@"Xbim.Geometry.Engine32.dll")]
        [DeploymentItem(@"Xbim.Geometry.Engine64.dll")]
        [DeploymentItem(@"Files\OneWallTwoWindows.ifc")]
        public void CanConvertOneFile()
        {
            var ifc = new FileInfo("OneWallTwoWindows.ifc");
            Assert.IsTrue(ifc.Exists);

            var xbim = CreateGeometry(ifc, true, false);

            using (var s = IfcStore.Open(xbim.FullName))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                var savename = Path.ChangeExtension(s.FileName, ".gltf");
                var bldr = new Builder();
                var ret = bldr.BuildInstancedScene(s, XbimMatrix3D.Identity);
                glTFLoader.Interface.SaveModel(ret, savename);

                Debug.WriteLine($"Gltf Model exported to '{savename}' in {sw.ElapsedMilliseconds} ms.");
                FileInfo f = new FileInfo(s.FileName);

                // write json
                //
                var jsonFileName = Path.ChangeExtension(s.FileName, "json");
                var bme = new Xbim.GLTF.SemanticExport.BuildingModelExtractor();
                var rep = bme.GetModel(s);
                rep.Export(jsonFileName);
            }
        }

        private static FileInfo CreateGeometry(FileInfo f, bool mode, bool useAlternativeExtruder)
        {
            //IfcStore.ModelProviderFactory.UseHeuristicModelProvider();
            XbimServices.Current.ConfigureServices(s => s.AddXbimToolkit(opt => opt.AddHeuristicModel()));

            using (var m = IfcStore.Open(f.FullName))
            {
                var c = new Xbim3DModelContext(m);
                c.CreateContext(null, mode);
                var newName = Path.ChangeExtension(f.FullName, mode + ".xbim");
                m.SaveAs(newName);
                return new FileInfo(newName);
            }
        }
    }
}
