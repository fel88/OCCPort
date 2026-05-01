#include "delabella.h"
#include <vector>
public ref class DelabellaWrapper {
	IDelaBella* db;
public:
	int  Triangulate(int points, array<double >^ xx, array<double>^ yy) {
		db=IDelaBella::Create();
		std::vector<double > xv;
		std::vector<double > yv;
		for (size_t i = 0; i < points; i++)
		{
			xv.push_back(xx[i]);
			yv.push_back(yy[i]);
		}
		
		double* x = &xv[0];
		double*  y = &yv[0];
		auto res=db->Triangulate(points, x, y, 2*sizeof(double));
		return res;
	 }
};