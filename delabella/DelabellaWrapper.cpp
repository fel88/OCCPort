#include "delabella.h"
#include <vector>

public ref class TVert {
public:int index;
	  double x;
	  double y;
};
public ref class TRet {
public:
	array<TVert^>^ v;
	TRet^ Next;
};
public ref class DelabellaWrapper {
	IDelaBella* db;
public:
	TRet^ GetFirstTriangle() {
		auto ret = db->GetFirstDelaunayTriangle();
		TRet^ ret1 = nullptr;
		TRet^ root = nullptr;
		while (ret) {
			auto old = ret1;
			ret1 = gcnew TRet();
			if (!root)
				root = ret1;
			if (old) {
				old->Next = ret1;
			}
			ret1->v = gcnew array<TVert^>(3);
			for (size_t i = 0; i < 3; i++)
			{
				ret1->v[i] = gcnew TVert();
				ret1->v[i]->x = ret->v[i]->x;
				ret1->v[i]->y = ret->v[i]->y;
				ret1->v[i]->index = ret->v[i]->i;

			}
			
			ret = ret->next;
		}
		return root;

	}
	int  Triangulate(int points, array<double >^ xx, array<double>^ yy) {
		db = IDelaBella::Create();
		
		std::vector<double> aPoints(2 * (points));
		for (size_t i = 0; i < points; i++)
		{
			aPoints[i * 2] = xx[i];
			aPoints[i * 2 + 1] = yy[i];		
		}		
		auto res = db->Triangulate(points, &aPoints[0], &aPoints[1], 2 * sizeof(double));
		
		return res;
	}
};