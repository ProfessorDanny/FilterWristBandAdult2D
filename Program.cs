// LRS Filter program for Arm Bands
// Basic function to cover printing the Arm Band



using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace FilterArmBand
{
    class Program
    {
        private static bool overwrite;

        static int Main(string[] args)
        {

            //Declare any variables in use
            int counter = 0;

            String InputFileName;
            String OutputFileName;
            //String AttrFileName;
            String line;
            String BarCodeVal = "";
            String TempFile;
            String term = "\r\n";

            string[] LabelLine = new string[14];
            string[] BarCodeData = new string[2];
            string Working1 = "";
            string ZPLString = "";

            //The filter definition should look like this
            //Datatype all
            //Command - filter.exe (this program)
            //Arguments: &infile &outfile &attrfile

            InputFileName = args[0];
            OutputFileName = args[1];
            //AttrFileName = args[2];


            //InputFileName = @"D:\test\Read4.txt";
            //OutputFileName = @"D:\test\WriteText1.txt";
            //AttrFileName = @"D:\test\AttrFileText.txt";


            //  The following is how to post information to the LRS Printer Log
            //  Console.WriteLine("<!VPSX-MsgType>INFO");
            //  Console.WriteLine("This is what the Info message type will look like"); 

            //  Console.WriteLine("<!VPSX-MsgType>WARN");
            //  Console.WriteLine("This is what the Warning message type will look like");

            //  Console.WriteLine("<!VPSX-MsgType>ERROR");
            //  Console.WriteLine("This is what the Error message type will look like");

            //  Console.WriteLine("<!VPSX-MsgType>DEBUG");
            //  Console.WriteLine("This is what the Debug message type will look like");

            Console.WriteLine("<!VPSX-MsgType>DEBUG");
            Console.WriteLine("Converting WristBand Data to ZPL");

            Console.WriteLine("<!VPSX-MsgType>INFO");
            Console.WriteLine("The Input Filename is {0}", InputFileName);
            Console.WriteLine("<!VPSX-MsgType>INFO");
            Console.WriteLine("The Output Filename is {0} ", OutputFileName);

            // Create a file to write to.


            using (StreamWriter sw = File.CreateText(OutputFileName))
            {
                sw.WriteLine(term);

                Console.WriteLine("<!VPSX-MsgType>DEBUG");
                Console.WriteLine("Create Output File");


            }

            TempFile = string.Format(@"D:\temp\{0}.TXT", Guid.NewGuid());

            File.Copy(InputFileName, TempFile);
            //Console.WriteLine(TempFile);

            //VPSX expects all filters to create an altered output file.  At this point
            //this filter could simply copy the input file to the output file


            // read data from input file
            // Read the file  
            System.IO.StreamReader file =
                new System.IO.StreamReader(TempFile);

            Console.WriteLine("<!VPSX-MsgType>DEBUG");
            Console.WriteLine("Open File for Read");


            while ((line = file.ReadLine()) != null)
            {
                char[] charArr = line.ToCharArray();

                if (line == "")
                {
                    int a;
                    System.Console.WriteLine(" BLANK Line ");
                }
                else
                {
                    if (line.Length < 2)
                    {
                        //Skip
                        Console.WriteLine("<!VPSX-MsgType>ERROR");
                        Console.WriteLine("Short Line Improper Data format Skip Line");
                    }
                    else
                    {
                        //System.Console.WriteLine(line);

                        LabelLine[counter] = line;  // will cause error for wrong data  ie laser job
                        counter++;

                    }


                    if (counter >= 6)
                    {//ERROR
                        Console.WriteLine("<!VPSX-MsgType>ERROR");
                        Console.WriteLine("More than 6 lines with no *");

                    }

                }


                foreach (char ch in charArr)
                {
                    // if (ch == 0x00)
                    //     System.Console.WriteLine(" NULL ");

                    // if (ch == 0x0A)
                    //     System.Console.WriteLine(" LF ");

                    if (ch == 0x2A || ch == 0X60) // end of data has * or tic
                    {


                        if (ch == 0x2A)
                        {
                            Console.WriteLine("<!VPSX-MsgType>DEBUG");
                            Console.WriteLine("End of Data * Found Write One ZPL Label to Output File");
                        }

                        if (ch == 0x60)
                        {
                            Console.WriteLine("<!VPSX-MsgType>DEBUG");
                            Console.WriteLine("End of Data TIC Found Write One ZPL Label to Output File");
                        }

                        //System.Console.WriteLine(" Form Feed ");
                        //NewLabel = 1;  // FF end of label 
                        counter = 0;
                        Working1 = LabelLine[1];
                        // Console.WriteLine("******************");
                        BarCodeData[0] = LabelLine[1].Substring(2, 8);
                        BarCodeVal = LabelLine[1].Substring(2, 8);
                        //Console.WriteLine(BarCodeVal);
                        //Console.WriteLine("******************");

                        //Console.Write("Line 3 :");
                        //Console.WriteLine(LabelLine[3]);

                        // Console.Write("Line 4 :");
                        //Console.WriteLine(LabelLine[4]);

                        if (ch == 0x60)
                        {
                            Console.WriteLine("Tic found");
                            if (LabelLine[3].Length > 15)
                            {
                                Console.WriteLine("Length > 15");
                                char[] charsToTrim = { '*', ' ', '\'' };
                                //Working1 = LabelLine[3].Trim(charsToTrim);

                                Working1 = LabelLine[3].Substring(36, 11);
                                LabelLine[3] = " " + Working1;

                            }


                        }
                        else
                        {
                            //if (counter > 3)
                            {
                                char[] charsToTrim = { '*', ' ' };
                                Working1 = LabelLine[4].Trim(charsToTrim);
                                LabelLine[4] = " " + Working1;
                            }
                        }

                        /*
                        Console.Write("Line 0 :"); 
                        Console.WriteLine(LabelLine[0]);

                        Console.Write("Line 1 :");
                        Console.WriteLine(LabelLine[1]);

                        Console.Write("Line 2 :");
                        Console.WriteLine(LabelLine[2]);

                        Console.Write("Line 3 :");
                        Console.WriteLine(LabelLine[3]);

                        Console.Write("Line 4 :");
                        Console.WriteLine(LabelLine[4]);
                        */
                        // Working1 = LabelLine[5].Trim(charsToTrim);
                        // LabelLine[5] = Working1;



                        // Data checks

                        // Start Build ZPL Code for Adult2D WristBand from data


                        // WriteLabel();


                        // need to check BarCodeVal in case there is a Canceled Visit (make sure it is numeric)
                        if (Regex.IsMatch(BarCodeVal, @"^\d+$"))
                        {
                            ZPLString =
                            "^XA" + term +
                            "^FT22,2200 ^A0B,30,36^FD" + LabelLine[0] + "^FS" + term +
                            "^FT52,2200 ^A0B,30,36^FD" + LabelLine[1] + "^FS" + term +
                            "^FT82,2200 ^A0B,30,36^FD" + LabelLine[2] + "^FS" + term +
                            "^FT112,2200 ^A0B,30,36^FD" + LabelLine[3] + "^FS" + term +
                            "^FT142,2200 ^A0B,30,36^FD" + LabelLine[4] + "^FS" + term +
                            "^FO153,1895^BY2^BCB,51,N,N^FDAC" + BarCodeVal + "^FS" + term +  // Code 128 Linear 

                            "^FO0,2230 ^BY4^BXB,4,200,14,14,4^FDAC" + BarCodeVal + "^FS" + term +  // Datamatrix code 1 of 4
                            "^FO0,1545 ^BY4^BXB,4,200,14,14,4^FDAC" + BarCodeVal + "^FS" + term +  // Datamatrix code 2 of 4
                            "^FO89,2230 ^BY4^BXB,4,200,14,14,4^FDAC" + BarCodeVal + "^FS" + term + // Datamatrix code 3 of 4
                            "^FO89,1545 ^BY4^BXB,4,200,14,14,4^FDAC" + BarCodeVal + "^FS" + term + // Datamatrix code 4 of 4
                            "^PQ1" +
                            "^XZ ^MCY" + term + term;

                            Console.WriteLine("<!VPSX-MsgType>DEBUG");
                            Console.WriteLine("Write One ZPL Label to Output File with Barcodes");

                        }
                        else //  BarCodeVal in non numeric  E.G. Canceled Visit
                        {
                            ZPLString =
                            "^XA" + term +
                            "^FT22,2200 ^A0B,30,36^FD" + LabelLine[0] + "^FS" + term +
                            "^FT52,2200 ^A0B,30,36^FD" + LabelLine[1] + "^FS" + term +
                            "^FT82,2200 ^A0B,30,36^FD" + LabelLine[2] + "^FS" + term +
                            "^FT112,2200 ^A0B,30,36^FD" + LabelLine[3] + "^FS" + term +
                            "^FT142,2200 ^A0B,30,36^FD" + LabelLine[4] + "^FS" + term +

                            //"^FO153,1895^BY2^BCB,51,N,N^FDAC" + BarCodeVal + "^FS" + term +  // Code 128 Linear 
                            //"^FO0,2230 ^BY4^BXB,4,200,14,14,4^FDAC" + BarCodeVal + "^FS" + term +  // Datamatrix code 1 of 4
                            //"^FO0,1545 ^BY4^BXB,4,200,14,14,4^FDAC" + BarCodeVal + "^FS" + term +  // Datamatrix code 2 of 4
                            //"^FO89,2230 ^BY4^BXB,4,200,14,14,4^FDAC" + BarCodeVal + "^FS" + term + // Datamatrix code 3 of 4
                            //"^FO89,1545 ^BY4^BXB,4,200,14,14,4^FDAC" + BarCodeVal + "^FS" + term + // Datamatrix code 4 of 4

                            "^PQ1" +
                            "^XZ ^MCY" + term + term;

                            Console.WriteLine("<!VPSX-MsgType>DEBUG");
                            Console.WriteLine("Canceled Visit  Write One ZPL Label to Output File with no Barcodes");


                        }



                        // End Wrist Band Print


                        using (StreamWriter sw = File.AppendText(OutputFileName))
                        {
                            sw.WriteLine(ZPLString);

                        }

                        // zero out label space

                        for (int i = 0; i <= 13; i++)
                        {
                            LabelLine[i] = "";
                        }
                    }

                    // if (ch == 0x0D)
                    //     System.Console.WriteLine(" CR ");

                }



            }

            // close the file?
            file.Close();

            //File.Delete(TempFile);

            Console.WriteLine("<!VPSX-MsgType>DEBUG");
            Console.Write("Done Close Working File and Delete:");

            //Console.WriteLine("<!VPSX-MsgType>DEBUG");
            Console.WriteLine(TempFile);

            // System.Console.WriteLine("There were {0} lines.", counter);
            // Suspend the screen.  






            //(StreamWriter w = File.AppendText("log.txt"))

            //VPSX also has Filter Feedback commands which can affect the behavior
            //of VPSX.  I will use those here

            // Console.WriteLine("<!VPSX-DoNotPrint>");  //We don't really want anything printed here
            //Console.WriteLine("<!VPSX-NoOutputFile>"); //Tell VPSX that there is no output file

            return 0;



        }

    }



}
