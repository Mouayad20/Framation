namespace OpenCvSharp.Demo
{
	using UnityEngine;
	using System.Collections;
	using OpenCvSharp;
	using UnityEngine.UI;
	using System.IO;
	using System.Collections.Generic;
	using Newtonsoft.Json;
	
	public class PointsToTriangulation
	{
		public List<PointXY> target { get; set; }
	}

	public class PointXY
	{
		public int x { get; set; }
		public int y { get; set; }
	}
 
	public class ContoursByShapeScript : MonoBehaviour {

		public Texture2D texture;
		[SerializeField] Material lineMat;

		// Use this for initialization
		void Start () {

			PointsToTriangulation pointsToTriangulation = new PointsToTriangulation();
			List<PointXY> pointXYList = new List<PointXY>();
	
			
			//Load texture
			Mat image = Unity.TextureToMat(this.texture);

			//Gray scale image
			Mat grayMat = new Mat();
			Cv2.CvtColor (image, grayMat, ColorConversionCodes.BGR2GRAY); 

			Mat thresh = new Mat ();
			Cv2.Threshold (grayMat, thresh, 127, 255, ThresholdTypes.BinaryInv);

			// Extract Contours
			Point[][] contours;
			HierarchyIndex[] hierarchy;
			Cv2.FindContours (thresh, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, null);
			/*	
				// newPoints = contours[0];
				// foreach (Point[] contour in contours) {
				// for (int k = 0 ; k < contours.Length - 1  ; k++){
				// 	newPoints[k] = contour[k+1]; 
				// }
				// 	print("__________contour  _________________  " + contour.Length );
				// 	print("__________newPoints________________  " + newPoints.Length );
				// 	foreach (Point point in contour) {
				// 		Cv2.Circle(image,point,2,new Scalar(0,0,255),-1);
				// 	}
				// 	double length = Cv2.ArcLength(contour, true);
				// 	Point[] approx = Cv2.ApproxPolyDP(contour, length * 0.01, true);
				// 	string shapeName = null;
				// 	Scalar color = new Scalar();

				// 	if (approx.Length == 3) {
				// 		shapeName = "Triangle";
				// 		color = new Scalar(0,255,0);
				// 	}
				// 	else if (approx.Length == 4) {
				// 		OpenCvSharp.Rect rect = Cv2.BoundingRect(contour);
				// 		if (rect.Width / rect.Height <= 0.1) {
				// 			shapeName = "Square";
				// 			color = new Scalar(0,125 ,255);
				// 		} else {
				// 			shapeName = "Rectangle";
				// 			color = new Scalar(0, 0 ,255);
				// 		}
				// 	}
				// 	else if (approx.Length == 10) {
				// 		shapeName = "Star";
				// 		color = new Scalar(255, 255, 0);
				// 	}
				// 	else if (approx.Length >= 15) {
				// 		shapeName = "Circle";
				// 		color = new Scalar(0, 255, 255);
				// 	}

				// 	if (shapeName != null) {
				// 		Moments m = Cv2.Moments(contour);
				// 		int cx = (int)(m.M10 / m.M00);
				// 		int cy = (int)(m.M01 / m.M00);

				// 		Cv2.DrawContours(image, new Point[][] {contour}, 0, color, -1);
				// 		Cv2.PutText(image, shapeName, new Point(cx-50, cy), HersheyFonts.HersheySimplex, 1.0, new Scalar(0, 0, 0));
				// 	}
				// }
			*/
			// Render texture
			Texture2D texture = Unity.MatToTexture (image);
			RawImage rawImage = gameObject.GetComponent<RawImage> ();
			rawImage.texture = texture;

			print("__________contour  _________________  " + contours[0].Length );
			
			for (int k = 0 ; k < contours[0].Length -2 ; k++){
				if (k % 15 == 0 ){
					PointXY p = new PointXY();
					p.x = contours[0][k+1].X; 
					p.y = -contours[0][k+1].Y; 
					pointXYList.Add(p);
				}else continue;
			}

			print("__________newPoints________________  " + pointXYList.Count );
			pointsToTriangulation.target = pointXYList;
			string json = JsonConvert.SerializeObject(pointsToTriangulation);
	
			File.WriteAllText(@"C:/Users/HP/Downloads/Compressed/trianglation/Assets/StreamingAssets/points.json", json);
		}

		// Update is called once per frame
		void Update () {
			
		}


	}
}