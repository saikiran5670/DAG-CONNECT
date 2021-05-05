package utiliities;

import java.io.FileInputStream;
import java.io.FileOutputStream;


import org.apache.poi.xssf.usermodel.XSSFWorkbook;

import objectProperties.Constants;

import org.apache.poi.xssf.usermodel.XSSFSheet;
import org.apache.poi.xssf.usermodel.XSSFRow;
import org.apache.poi.xssf.usermodel.XSSFCell;

public class ExcelSheet 
{
	public static XSSFWorkbook ExcelLWBook;
	public static XSSFSheet ExcelLWSheet;
	public static XSSFRow Row;
	public static XSSFCell Cell;
	
//*******************************************************************************	
 public static void setExccelFile (String Path) 
 {
	//System.out.println(Path);
	FileInputStream ExcelFile;
	
	try 
	{
		ExcelFile = new FileInputStream(Path);
	    ExcelLWBook = new XSSFWorkbook(ExcelFile);
	} 
	catch (Exception e) 
	{
		// TODO Auto-generated catch block
		e.printStackTrace();
	}
}
//*******************************************************************************	

 public static int getRowCount (String SheetName) 
 {
   	int iNumber = 0;
   	try 
   	{
    	ExcelLWSheet = ExcelLWBook.getSheet(SheetName);
    	iNumber = ExcelLWSheet.getLastRowNum()+1;
    	 
   	} 
   	catch (Exception e) 
    {
    	// TODO Auto-generated catch block
    	e.printStackTrace();
    }
    return iNumber;
 }
//*******************************************************************************	
    
 public static String getCellData (int Rownumber, int Columnnumber, String SheetName) 
 {
  	try 
    {
        ExcelLWSheet = ExcelLWBook.getSheet(SheetName);
    	Cell = ExcelLWSheet.getRow(Rownumber).getCell(Columnnumber);
    	String CellData = Cell.getStringCellValue();
    	return CellData;
   	} 
  	catch (Exception e) 
    {
        // TODO Auto-generated catch block
    	e.printStackTrace();
    	return " ";
    }
 }
//*******************************************************************************	
 
 public static int getRowContains(String TestCaseName, int colNum, String SheetName) 
 {
	int iRowNum=0;
	
	try 
	{
		int rowCount = ExcelSheet.getRowCount(SheetName);
		for (; iRowNum<rowCount;iRowNum++)
	{
		if (ExcelSheet.getCellData(iRowNum, colNum, SheetName).equalsIgnoreCase(TestCaseName))
	{
		break;
	}
	}
	}
	catch (Exception e) 
	{
		// TODO Auto-generated catch block
		e.printStackTrace();
	}
	return iRowNum;
 }
//*******************************************************************************	
 
 public static int getStepCount(String SheetName, String TestCaseID, int TestCaseStart)
 {
	try 
	{
		for (int i=TestCaseStart; i<=ExcelSheet.getRowCount(SheetName);i++)
	{
		if(!TestCaseID.equals(ExcelSheet.getCellData(i, Constants.Col_TestCaseID, SheetName)))
	{
		int number = i;
		return number;
	}
    }
		ExcelLWSheet = ExcelLWBook.getSheet(SheetName);
		int number = ExcelLWSheet.getLastRowNum()+1;
		return number;
	}
	catch (Exception e) 
	{
		// TODO Auto-generated catch block
		e.printStackTrace();
		return 0;
	}
}
//*******************************************************************************	
 public static void setCellData (String Result, int Rownum, int ColNum, String SheetName)
{
	try 
	{
		ExcelLWSheet = ExcelLWBook.getSheet(SheetName);
		Row = ExcelLWSheet.getRow(Rownum);
		Cell = Row.getCell(ColNum);
		if (Cell == null) 
		{
			Cell = Row.createCell(ColNum);
			Cell.setCellValue(Result);
		}
		else 
		{
			Cell.setCellValue(Result);
		}
		
		String localDir = System.getProperty("user.dir");
		FileOutputStream fileOut = new FileOutputStream(localDir + Constants.Path_TestData);
		ExcelLWBook.write(fileOut);
		fileOut.close();
		
		ExcelLWBook = new XSSFWorkbook(new FileInputStream(localDir + Constants.Path_TestData) );
	    }
	catch (Exception e) 
	{
		// TODO Auto-generated catch block
		e.printStackTrace();
				
	}
}




}
