#pragma once
#define DLL_API extern "C" _declspec(dllexport)
#include "opencv2/highgui/highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include "cv.h"
#include <iostream>
#include <fstream>

#define pi 3.1415926
#define WL 0.6328
using namespace std ;
using namespace cv;

DLL_API void SetStd(float* imgstd, int width, int height, int step, int channels);
DLL_API uchar* GetStd(float* img, int width, int height, int step, int channels,int& steps);