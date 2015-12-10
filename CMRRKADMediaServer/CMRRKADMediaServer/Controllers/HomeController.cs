using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using Amazon.S3.Transfer;

namespace CMRRKADMediaServer.Controllers
{
    public class HomeController : Controller
    {
        static IAmazonS3 client;
       static string accessKey = "AKIAIBXPM2A2QLSRWZIA";
       static string secretKey = "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";
       static string bucketName = @"vtest123";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpGet]
        public ActionResult UploadFile()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult UploadDirectory()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.test = 2.3;
            return View();
        }

        public ActionResult DownloadDirectory()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile(HttpPostedFileBase fileS3)
        {
            //HttpPostedFileBase file = Request.Files[0];
            // if (fileS3.ContentLength > 0) // accept the file
            // {
          

         
            string keyName = "testfile";
            //string filePath = "*** absolute path to a sample file to upload ***";
            // var fileName= Path.GetFullPath(fileS3.FileName);
            //var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
            //var fileName = Server.MapPath(fileS3.FileName);
            string fileName = fileS3.FileName;
            using (client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.EUWest1))
            {


                ListObjectsRequest listObjectRequest =
                                      new ListObjectsRequest();
                listObjectRequest.BucketName = bucketName;

                // Display Object from to Amazon S3.
                ListObjectsResponse response = client.ListObjects(listObjectRequest);
                List<S3Object> objects = response.S3Objects;

                ViewBag.BucketObjects = objects.Count;
                foreach (var item in objects)
                {
                    ViewBag.BucketObjects += item.Key;
                }

                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = bucketName,
                    Key = keyName,
                    FilePath = fileName
                };
                PutObjectResponse response2 = client.PutObject(request);

            }

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadDirectory(string txtDirectoryName, string txtBucketName)
        {
            try
            {
                //TransferUtility directoryTransferUtility = new TransferUtility(new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.EUWest1));


                //    // 1. Upload a directory.
                //    txtDirectoryName = txtDirectoryName.Trim();
                //    txtBucketName = txtBucketName.Trim();
                //    directoryTransferUtility.UploadDirectory(txtDirectoryName, bucketName, "*.*", SearchOption.AllDirectories);

                txtDirectoryName = txtDirectoryName.Trim();
                txtBucketName = txtBucketName.Trim();

                using (TransferUtility directoryTransferUtility = new TransferUtility(new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.EUWest1)))
                {
                    var transferRequest = new TransferUtilityUploadDirectoryRequest
                    {

                        BucketName = bucketName,
                        Directory = txtDirectoryName,
                        SearchPattern = "*.*",
                        SearchOption = SearchOption.AllDirectories

                    };
                    transferRequest.UploadDirectoryProgressEvent += TransferRequestOnUploadDirectoryProgressEvent;

                    directoryTransferUtility.UploadDirectory(transferRequest);
                   
                };
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(e.Message, e.InnerException);
            }

            return View();
        }

        private void TransferRequestOnUploadDirectoryProgressEvent(object sender, UploadDirectoryProgressArgs e)
        {
            // Process event.
            // Console.WriteLine("{0}/{1}", e.TransferredBytes, e.TotalBytes);
            ViewBag.Status = e.TransferredBytes + "dd" + e.TotalBytes;
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DownloadDirectory(string txtDirectory, string txtBucketName,string txtlocalDrive)
        {
            try
            {
                TransferUtility directoryTransferUtility = new TransferUtility(new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.EUWest1));

                // 1. Upload a directory.
                txtDirectory = txtDirectory.Trim();
                txtBucketName = txtBucketName.Trim();
                txtlocalDrive = txtlocalDrive.Trim();
                directoryTransferUtility.DownloadDirectoryAsync(txtBucketName, txtDirectory, txtlocalDrive);
            }


            catch (AmazonS3Exception e)
            {
                Console.WriteLine(e.Message, e.InnerException);
            }

            return View();
        }
    }
}