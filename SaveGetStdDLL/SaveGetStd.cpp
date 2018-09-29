#include "SaveGetStd.h"

void SetStd(float* imgstd, int width, int height, int step, int channels)
{
	Mat Std = Mat(height, width, CV_32FC1, imgstd, step);
	FileStorage fs("F:\\tempstd.xml", FileStorage::WRITE);
	write(fs, "std", Std);
    fs.release();
}
uchar* GetStd(float* img, int width, int height, int step, int channels,int& steps)
{
	FileStorage fs("F:\\tempstd.xml", FileStorage::READ);
	Mat Std(height, width, CV_32FC1);
	fs["std"] >> Std;
	fs.release();
	Mat Img = Mat(height, width, CV_32F, img, step);
	Mat real = Img - Std;
	double max, min;
	minMaxLoc(real, &min, &max);
	float scale = 255 / (max - min);
	float shift = 255 * min / (min - max);
	Mat outReal(height, width, CV_8UC1);
	real.convertTo(outReal, CV_8UC1, scale, shift);
	IplImage* dst = &IplImage(outReal);
	IplImage* dstres = cvCreateImage(cvSize(width, height), 8, 1);
	cvCopy(dst, dstres);
	Mat tempDst(dstres);
	uchar* REALRES = new uchar[width*height];
	REALRES = tempDst.data;
	steps = tempDst.step[0];
	return REALRES;
}