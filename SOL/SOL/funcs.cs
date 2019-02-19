using System.Diagnostics;
using System.IO;

namespace SOL
{
    class funcs
    {
        //funcs is for storing non-static functions

        FileInfo fileI;
        bool prcMsg = false;

        /// <summary>
        /// checks if the file is currently in use, returns true if it is not.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
		protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);//try and open the file to force throw catch
                stream.Flush();//kill the stream's info

            }
            catch (IOException)
            {
                Debug.WriteLine("exception caught");

                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();

            }

            return false;
        }   //https://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use

        /// <summary>
        /// Sends a string of data to a file for logging and debugging.
        /// </summary>
        /// <param name="logToSend"></param>
        /// <param name="path"></param>
		public void log(string logToSend, string path)
        {

            fileI = new FileInfo(path);

            if (!logToSend.Equals(""))
            {
                try
                {

                    //Debug.WriteLine("");

                    //Debug.WriteLine("tring to send the message \"" + logToSend + "\" ");

                    if (File.Exists(path))
                    {
                        if (!IsFileLocked(fileI))
                        {
                            TextWriter tsw = new StreamWriter(path, true);
                            tsw.WriteLine(logToSend);
                            tsw.Close();
                            tsw = null;
                            prcMsg = false;
                            //Debug.WriteLine("sucessfully sent message \"" + logToSend + "\" ");
                        }
                        else
                        {
                            Debug.WriteLine("Could not send the error \"" + logToSend + "\" to error log!");
                            do
                            {
                                Debug.WriteLine("retrying to send log " + logToSend);
                                prcMsg = true;
                                if (!IsFileLocked(fileI))
                                {
                                    TextWriter tsw = new StreamWriter(path, true);
                                    tsw.WriteLine(logToSend);
                                    tsw.Close();
                                    tsw = null;
                                    Debug.WriteLine("retry completed, \"" + logToSend + "\" sent.");
                                    prcMsg = false;
                                }
                                if (prcMsg)
                                {
                                    Debug.WriteLine("failed to send \"" + logToSend + "\"!");
                                }
                            } while (IsFileLocked(fileI));
                        }
                    }

                }
                catch (IOException)
                {
                    Debug.WriteLine("exception caught, message \"" + logToSend + "\" failed to send. Retrying.");
                    prcMsg = true;
                    log(logToSend, path);

                }

            }
            else
            {

                try
                {

                    Debug.WriteLine("");

                    if (File.Exists(path))
                    {
                        if (!IsFileLocked(fileI))
                        {
                            TextWriter tsw = new StreamWriter(path, true);
                            tsw.WriteLine("");
                            tsw.Close();
                            tsw = null;
                            prcMsg = false;
                        }
                        else
                        {
                            do
                            {
                                prcMsg = true;
                                if (!IsFileLocked(fileI))
                                {
                                    TextWriter tsw = new StreamWriter(path, true);
                                    tsw.WriteLine("");
                                    tsw.Close();
                                    tsw = null;
                                    prcMsg = false;
                                }
                                if (prcMsg)
                                {
                                }
                            } while (IsFileLocked(fileI));
                        }
                    }

                }
                catch (IOException)
                {
                    Debug.WriteLine("exception caught, message \"" + logToSend + "\" failed to send. Retrying.");
                    prcMsg = true;
                    log(logToSend, path);

                }

            }

        }

    }
}
