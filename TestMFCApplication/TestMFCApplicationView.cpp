
// TestMFCApplicationView.cpp : implementation of the CTestMFCApplicationView class
//

#include "stdafx.h"
// SHARED_HANDLERS can be defined in an ATL project implementing preview, thumbnail
// and search filter handlers and allows sharing of document code with that project.
#ifndef SHARED_HANDLERS
#include "TestMFCApplication.h"
#endif

#include "TestMFCApplicationDoc.h"
#include "TestMFCApplicationView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CTestMFCApplicationView

IMPLEMENT_DYNCREATE(CTestMFCApplicationView, CView)

BEGIN_MESSAGE_MAP(CTestMFCApplicationView, CView)
	// Standard printing commands
	ON_COMMAND(ID_FILE_PRINT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &CTestMFCApplicationView::OnFilePrintPreview)
	ON_WM_CONTEXTMENU()
	ON_WM_RBUTTONUP()
END_MESSAGE_MAP()

// CTestMFCApplicationView construction/destruction

CTestMFCApplicationView::CTestMFCApplicationView()
{
	// TODO: add construction code here

}

CTestMFCApplicationView::~CTestMFCApplicationView()
{
}

BOOL CTestMFCApplicationView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: Modify the Window class or styles here by modifying
	//  the CREATESTRUCT cs

	return CView::PreCreateWindow(cs);
}

// CTestMFCApplicationView drawing

void CTestMFCApplicationView::OnDraw(CDC* /*pDC*/)
{
	CTestMFCApplicationDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	// TODO: add draw code for native data here
}


// CTestMFCApplicationView printing


void CTestMFCApplicationView::OnFilePrintPreview()
{
#ifndef SHARED_HANDLERS
	AFXPrintPreview(this);
#endif
}

BOOL CTestMFCApplicationView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// default preparation
	return DoPreparePrinting(pInfo);
}

void CTestMFCApplicationView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: add extra initialization before printing
}

void CTestMFCApplicationView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: add cleanup after printing
}

void CTestMFCApplicationView::OnRButtonUp(UINT /* nFlags */, CPoint point)
{
	ClientToScreen(&point);
	OnContextMenu(this, point);
}

void CTestMFCApplicationView::OnContextMenu(CWnd* /* pWnd */, CPoint point)
{
#ifndef SHARED_HANDLERS
	theApp.GetContextMenuManager()->ShowPopupMenu(IDR_POPUP_EDIT, point.x, point.y, this, TRUE);
#endif
}


// CTestMFCApplicationView diagnostics

#ifdef _DEBUG
void CTestMFCApplicationView::AssertValid() const
{
	CView::AssertValid();
}

void CTestMFCApplicationView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

CTestMFCApplicationDoc* CTestMFCApplicationView::GetDocument() const // non-debug version is inline
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CTestMFCApplicationDoc)));
	return (CTestMFCApplicationDoc*)m_pDocument;
}
#endif //_DEBUG


// CTestMFCApplicationView message handlers
