using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace Mass_Renamer
{
    class ErrorLogger
    {
        //this class is for handling error reporting to external files (text documents) and other such reporting

        private FileInfo fileI;
        private bool prcMsg = false;
        private uint releasedLine = 0;//stores how many times a queue has been released but not reset
        private static object locker = new object();
        private static object locker2 = new object();
        private static object locker3 = new object();
        private string[][] queue = new string[0][];
        //[][0] = date
        //[][1] = caller
        //[][2] = lineNumber
        //[][3] = the log to send
        //[][4] = CallerFloatLength
        //[][5] = LineFloatLength
        //[][6] = filesFullPath
        //[][7] = disableExtra

        /// <summary>
        /// checks if the file is currently in use, returns true if it is.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
		public virtual bool IsFileLocked(FileInfo file)
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
        /// Forces the queue to re-tab the logs.
        /// </summary>
        public void ForceReTabbing(int tabRatio) {
            int pLength = 0;
            int li = -1;
            int existingTabCount = 0;

            //remove all tabbing

            if (queue.Length > 1)
            {
                for (int i = 0; i < queue.Length; i++)
                {//loop until reach this queued message (the last queued message)
                    string tmpa = queue[i][1].Replace("\t", "");
                    string tmpb = queue[i][2].Replace("\t", "");

                    int length = queue[i][0].Length + tmpa.Length + tmpb.Length;

                    if (length > pLength)
                    {
                        li = i;//get the queue with the largest caller name and\or lineNumber
                    }

                    pLength = queue[i][0].Length + tmpa.Length + tmpb.Length;
                }

                foreach (string[] item in queue)
                {//this is probably causing issues
                    existingTabCount = queue[li][1].Split('\t').Length - 1;
                    float CallerFloatLength = (float)(queue[li][1].Replace("\t", "").Length / tabRatio) + existingTabCount;//get the tab length of the caller of the largest queue

                    existingTabCount = queue[li][2].Split('\t').Length - 1;
                    float LineFloatLength = (float)(queue[li][2].Replace("\t", "").Length / tabRatio) + existingTabCount;//get the tab length of the @L of the largest queue

                    int TabLength = (int)Math.Floor(CallerFloatLength);//get the tab length of the largest queues' caller text as litteral
                    int OtherTabLength = (int)Math.Floor(decimal.Parse(item[4]));//get the tab length of the other caller text as litteral

                    if (TabLength > OtherTabLength)
                    {//if the caller name is longer than the other caller name by at least 1 tab
                     //append extra tabs to this items' caller text (after)
                        int diff = TabLength - OtherTabLength;
                        for (int i = 0; i < diff; i++)
                        {
                            item[1] = item[1] + "\t";

                        }

                    }
                    else if (TabLength < OtherTabLength)
                    {
                        //append extra tabs to the largest queue caller text (after)
                        int diff = OtherTabLength - TabLength;
                        for (int i = 0; i < diff; i++)
                        {
                            queue[li][1] = queue[li][1] + "\t";

                        }

                    }

                    //handling tabbing for @L(lineNumber)
                    TabLength = (int)Math.Floor(LineFloatLength);//get the tab length of the largest queues' @L text
                    int CharLength = (int)((float)((LineFloatLength - TabLength)) * 4);//get how many characters into the next tab the text is in, 0.75=3, 0.5=2, 0.25=1

                    existingTabCount = item[2].Split('\t').Length - 1;
                    OtherTabLength = (int)Math.Floor(decimal.Parse(item[5])) + existingTabCount;//get the tab length of the other @L text
                    int OtherCharLength = (int)((float)((decimal.Parse(item[5]) - (OtherTabLength - existingTabCount))) * 4);//0.75=3, 0.5=2, 0.25=1

                    if (TabLength > OtherTabLength)
                    {//if the @L full text is longer than the other caller full text by at least 1 tab
                     //append extra tabs to the OTHER @L full text (after)
                        int diff = TabLength - OtherTabLength;
                        for (int i = 0; i < diff; i++)
                        {
                            item[2] = item[2] + "\t";

                        }

                    }

                    existingTabCount = item[1].Split('\t').Length - 1;
                    item[4] = ((float)(item[1].Replace("\t", "").Length / tabRatio) + existingTabCount).ToString();//update tab count to account for the newly added tabbings

                    existingTabCount = item[2].Split('\t').Length - 1;
                    item[5] = ((float)(item[2].Replace("\t", "").Length / tabRatio) + existingTabCount).ToString();
                }
            }

        }

        /// <summary>
        /// Removes the last item in the queue.
        /// </summary>
        public void RemoveItemFromQueue() {
            if (queue.Length > 0)
            {
                queue[queue.Length -1] = null;
                Array.Resize(ref queue, queue.Length - 1);
            }

        }

        /// <summary>
        /// Removes an item at the defined index possition.
        /// </summary>
        /// <param name="poss"></param>
        public void RemoveItemFromQueue(int poss)
        {
            if (queue.Length > 0)
            {
                queue[poss] = null;

                for (int i = poss; i < queue.Length - 1; i++)
                {
                    queue[i] = queue[i + 1];

                }

                queue[queue.Length - 1] = null;
                Array.Resize(ref queue, queue.Length - 1);
            }

        }

        /// <summary>
        /// Removes a queue item from the queue which most closely matches the item searched for. 
        /// If this doesn't find any queue items which match the description, returns false.
        /// </summary>
        /// <param name="itemToSearchFor"></param>
        /// <param name="earliestOrLatest">Weather to search for the earliest (last) or latest (first) occurance of the searched queue item.</param>
        /// <returns></returns>
        public bool RemoveItemFromQueue(string[] itemToSearchFor, bool earliestOrLatest = true)
        {
            int index = GetIndexOfQueueItem(itemToSearchFor, earliestOrLatest);
            if (index != -1)
            {
                RemoveItemFromQueue(index);
                return true;
            }
            else {
                return false;

            }

        }

        /// <summary>
        /// Gets the queue index of the first or last occurance which most closely matches the item searched for. If none found, this will return -1.
        /// </summary>
        /// <param name="itemToSearchFor">The queue item to search for.</param>
        /// <param name="earliestOrLatest">Weather to search for the earliest (last) or latest (first) occurance.</param>
        /// <returns></returns>
        public int GetIndexOfQueueItem(string[] itemToSearchFor, bool earliestOrLatest = true)
        {
            int[] result = new int[8];

            for (ushort i = 0; i < result.Length; i++)
            {
                result.SetValue(-1, i);
            }

            //get the possition of the right most (last/earliest) occurance of each queue value to search
            for (int i1 = 0; i1 < queue.Length; i1++)
            {
                for (int i = 0; i < itemToSearchFor.Length; i++)
                {
                    if (itemToSearchFor[i] != null)
                    {
                        if (queue[i1][i].Replace("\t", "") == itemToSearchFor[i])
                        {
                            //get last occurance
                            result[i] = i1;//got a result for this type(i) on this(i1) item

                        }
                    }

                }

            }

            if (earliestOrLatest)
            {
                int res = result.GroupBy(item => item).OrderByDescending(g => g.Count()).Select(g => g.Key).First();//set the result array as a group and order the result so that the most often occuring number 'floats' to the top (first)

                if (res == -1)
                {
                    System.Collections.Generic.List<int> tmp = result.GroupBy(item => item).OrderByDescending(g => g.Count()).Select(g => g.Key).ToList();
                    tmp.RemoveAll(item => item == -1);

                    return tmp.First();

                }
                else
                {
                    return res;

                }
            }
            else
            {
                int res = result.GroupBy(item => item).OrderByDescending(g => g.Count()).Select(g => g.Key).Last();//set the result array as a group and order the result so that the most often occuring number 'floats' to the top (first)

                if (res == -1)
                {
                    System.Collections.Generic.List<int> tmp = result.GroupBy(item => item).OrderByDescending(g => g.Count()).Select(g => g.Key).ToList();
                    tmp.RemoveAll(item => item == -1);

                    return tmp.Last();

                }
                else
                {
                    return res;

                }
            }

        }

        /// <summary>
        /// Appends another queue into the possition of the current queue. Returns false if an error occured while appending the queue. Multi-threading freindly.
        /// </summary>
        /// <param name="queueToAppend"></param>
        /// <param name="possition"></param>
        /// <param name="disableWarnings">Weather to disable the warnings that this function produces on an error.</param>
        /// <param name="forceReTabbing">Weather to re-tab the logs or not (calls ForceReTabbing())</param>
        /// <param name="tabRatio">The amount of characters to tab ratio. Only takes affect if forceReTabbing is true.</param>
        /// <returns></returns>
        public bool AppendQueue(string[][] queueToAppend, uint possition, bool disableWarnings = true, bool forceReTabbing = true, int tabRatio = 4)
        {

            System.Threading.Monitor.Enter(locker3);//lock this function to process on one thread at a time
            bool valid = true;

            //check if the queue data is valid
            for (int i = 0; i < queueToAppend.Length; i++)
            {
                for (int i1 = 0; i1 < queueToAppend[i].Length; i1++)
                {
                    if (queueToAppend[i][i1] == null) {
                        valid = false;
                        if (disableWarnings)
                        {
                            MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][" + i1 + "] is null");
                        }
                        break;
                    }

                }

                try
                {
                    DateTime.Parse(queueToAppend[i][0]);//check if the date store is in fact a date
                    int.Parse(queueToAppend[i][2].Replace("@L", "").Replace(":", ""));//check if the line number store is in fact a number
                    float.Parse(queueToAppend[i][4]);//check if the Caller tab length (and over character count as a decimal) store is in fact a float
                    float.Parse(queueToAppend[i][5]);//check if the line count tab length (and over character count as a decimal) store is in fact a float
                    bool.Parse(queueToAppend[i][7]);//check if the option to disable extra text is a boolean

                    //check if the path is valid
                    foreach (char invalidC in Path.GetInvalidPathChars())
                    {
                        if (queueToAppend[i][6].Contains(invalidC.ToString())) {
                            valid = false;
                            if (disableWarnings)
                            {
                                MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][6] (fileFullPath) contains invalid path characters.");
                            }
                            break;
                        }
                    }

                    //check if the referenced file in the above path is valid
                    foreach (char invalidC in Path.GetInvalidFileNameChars())
                    {
                        if (queueToAppend[i][6].Contains(invalidC.ToString()))
                        {
                            valid = false;
                            if (disableWarnings)
                            {
                                MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][6] (fileFullPath) contains invalid file name characters.");
                            }
                            break;
                        }

                    }
                }
                catch {
                    valid = false;
                    bool whichIsIt = false;
                    DateTime dateDud;
                    int intDud;
                    float floatDud;
                    bool boolDud;

                    whichIsIt = DateTime.TryParse(queueToAppend[i][0], out dateDud);
                    if (!whichIsIt && disableWarnings)
                    {
                        MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][0]" +
                            " (DateTime) is not of its valid type.\n" + queueToAppend[i][0]);
                    }

                    whichIsIt = int.TryParse(queueToAppend[i][2], out intDud);
                    if (!whichIsIt && disableWarnings)
                    {
                        MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][2]" +
                            " (line number) is not of its valid type.\n" + queueToAppend[i][2].Replace("@L", "").Replace(":", ""));
                    }

                    whichIsIt = float.TryParse(queueToAppend[i][4], out floatDud);
                    if (!whichIsIt && disableWarnings)
                    {
                        MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][4] " +
                            "(caller tab length) is not of its valid type.\n" + queueToAppend[i][4]);
                    }

                    whichIsIt = float.TryParse(queueToAppend[i][5], out floatDud);
                    if (!whichIsIt && disableWarnings)
                    {
                        MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][5] " +
                            "(line number tab length) is not of its valid type.\n" + queueToAppend[i][5]);
                    }

                    whichIsIt = bool.TryParse(queueToAppend[i][7], out boolDud);
                    if (!whichIsIt && disableWarnings)
                    {
                        MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][7] " +
                            "(disableExtra) is not of its valid type.\n" + queueToAppend[i][7]);
                    }

                }

                if (!valid) { break; }

            }

            if (valid) {
                string[][] tmpQueue = new string[queueToAppend.Length + queue.Length][];
                for (int i = 0; i < queueToAppend.Length + queue.Length; i++)
                {
                    if (i <= possition)
                    {
                        tmpQueue[i] = queue[i];//fill the tmp queue to the point where the other queue will be appended into

                    }

                    if (i <= queueToAppend.Length + possition && i > possition)
                    {
                        tmpQueue[i] = queueToAppend[i - (possition + 1)];//append the other queue into the tmp queue
                    }
                    else if (i > (queueToAppend.Length - 1) + possition && i > possition && possition != queue.Length) {
                        tmpQueue[i] = queue[i - queueToAppend.Length];//append the rest of the main queue into the tmp queue

                    }

                }

                queue = tmpQueue;

            }

            if (forceReTabbing) {
                ForceReTabbing(tabRatio);
            }
            System.Threading.Monitor.Exit(locker3);//lock this function to process on one thread at a time
            return valid;

        }

        /// <summary>
        /// Appends another queue to the end of the current queue. Returns false if an error occured while appending the queue. Multi-threading freindly.
        /// </summary>
        /// <param name="queueToAppend"></param>
        /// <param name="disableWarnings"></param>
        /// <param name="forceReTabbing"></param>
        /// <param name="tabRatio"></param>
        /// <returns></returns>
        public bool AppendQueue(string[][] queueToAppend, bool disableWarnings = true, bool forceReTabbing = true, int tabRatio = 4)
        {

            System.Threading.Monitor.Enter(locker2);//lock this function to process on one thread at a time
            bool valid = true;

            //check if the queue data is valid
            for (int i = 0; i < queueToAppend.Length; i++)
            {
                for (int i1 = 0; i1 < queueToAppend[i].Length; i1++)
                {
                    if (queueToAppend[i][i1] == null)
                    {
                        valid = false;
                        if (disableWarnings)
                        {
                            MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][" + i1 + "] is null");
                        }
                        break;
                    }

                }

                try
                {
                    DateTime.Parse(queueToAppend[i][0]);//check if the date store is in fact a date
                    int.Parse(queueToAppend[i][2].Replace("@L", "").Replace(":", ""));//check if the line number store is in fact a number
                    float.Parse(queueToAppend[i][4]);//check if the Caller tab length (and over character count as a decimal) store is in fact a float
                    float.Parse(queueToAppend[i][5]);//check if the line count tab length (and over character count as a decimal) store is in fact a float
                    bool.Parse(queueToAppend[i][7]);//check if the option to disable extra text is a boolean

                    //check if the path is valid
                    foreach (char invalidC in Path.GetInvalidPathChars())
                    {
                        if (queueToAppend[i][6].Contains(invalidC.ToString()))
                        {
                            valid = false;
                            if (disableWarnings)
                            {
                                MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][6] (fileFullPath) contains invalid path characters.");
                            }
                            break;
                        }
                    }

                    //check if the referenced file in the above path is valid
                    foreach (char invalidC in Path.GetInvalidFileNameChars())
                    {
                        if (queueToAppend[i][6].Contains(invalidC.ToString()))
                        {
                            valid = false;
                            if (disableWarnings)
                            {
                                MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][6] (fileFullPath) contains invalid file name characters.");
                            }
                            break;
                        }

                    }
                }
                catch
                {
                    valid = false;
                    bool whichIsIt = false;
                    DateTime dateDud;
                    int intDud;
                    float floatDud;
                    bool boolDud;

                    whichIsIt = DateTime.TryParse(queueToAppend[i][0], out dateDud);
                    if (!whichIsIt && disableWarnings)
                    {
                        MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][0]" +
                            " (DateTime) is not of its valid type.\n" + queueToAppend[i][0]);
                    }

                    whichIsIt = int.TryParse(queueToAppend[i][2], out intDud);
                    if (!whichIsIt && disableWarnings)
                    {
                        MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][2]" +
                            " (line number) is not of its valid type.\n" + queueToAppend[i][2].Replace("@L", "").Replace(":", ""));
                    }

                    whichIsIt = float.TryParse(queueToAppend[i][4], out floatDud);
                    if (!whichIsIt && disableWarnings)
                    {
                        MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][4] " +
                            "(caller tab length) is not of its valid type.\n" + queueToAppend[i][4]);
                    }

                    whichIsIt = float.TryParse(queueToAppend[i][5], out floatDud);
                    if (!whichIsIt && disableWarnings)
                    {
                        MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][5] " +
                            "(line number tab length) is not of its valid type.\n" + queueToAppend[i][5]);
                    }

                    whichIsIt = bool.TryParse(queueToAppend[i][7], out boolDud);
                    if (!whichIsIt && disableWarnings)
                    {
                        MessageBox.Show("Failed to append queue to the current queue because the input queue[" + i + "][7] " +
                            "(disableExtra) is not of its valid type.\n" + queueToAppend[i][7]);
                    }

                }

                if (!valid) { break; }

            }

            if (valid)
            {
                int possition = queue.Length-1;

                if (possition > -1)
                {
                    string[][] tmpQueue = new string[queueToAppend.Length + queue.Length][];

                    for (int i = 0; i < queueToAppend.Length + queue.Length; i++)
                    {
                        if (i <= possition)
                        {
                            tmpQueue[i] = queue[i];//fill the tmp queue to the point where the other queue will be appended into

                        }

                        if (i <= queueToAppend.Length + possition && i > possition)
                        {
                            tmpQueue[i] = queueToAppend[i - (possition+1)];//append the other queue into the tmp queue
                        }

                    }

                    queue = tmpQueue;
                }
                else {
                    queue = queueToAppend;

                }

            }

            if (forceReTabbing)
            {
                ForceReTabbing(tabRatio);
            }
            System.Threading.Monitor.Exit(locker2);//lock this function to process on one thread at a time
            return valid;


        }

        /// <summary>
        /// Gets the current queues' size.
        /// </summary>
        /// <returns></returns>
        public int GetQueueSize() {
            return queue.Length;
        }

        /// <summary>
        /// Returns the current queues' data.
        /// </summary>
        /// <returns></returns>
        public string[][] GetQueue() {
            return queue;
        }

        /// <summary>
        /// Returns a certain log from the current queue.
        /// </summary>
        /// <param name="queueIndex"></param>
        /// <returns></returns>
        public string[] GetQueueItem(int queueIndex) {
            return queue[queueIndex];
        }

        /// <summary>
        /// Resets the current queue of QueueLog().
        /// </summary>
        public void ResetQueue() {
            queue = new string[0][];

        }

        /// <summary>
        /// Sends the logs, held by the queueLog(), function to the target file and resets the queue. Multi-Threading freindly.
        /// </summary>
        /// <param name="defaultFullPath">The default full file path to send the released queued text to if, a 
        /// path was not defined by the QueueLog() function.</param>
        /// <param name="resetQueue">Weather to reset the queue or not.</param>
        /// <param name="releasePrevious">Weather to release the same queue that was previously released, if any.</param>
        public void ReleaseQueue(string defaultFullPath, bool resetQueue = true, bool releasePrevious = false) {

            if (System.Threading.Thread.CurrentThread.IsBackground) {
                //use this in the process of renaming so that it is possible to pause, not stop
                //, the renaming process when the user presses cancel - so that a verification window can appear without issue.
                //ManualResetEvent syncEvent = new ManualResetEvent(false);
                //syncEvent.WaitOne();

                //https://www.c-sharpcorner.com/UploadFile/1d42da/threading-with-monitor-in-C-Sharp/
                System.Threading.Monitor.Enter(locker);//lock this function to process on one thread at a time
            }

            if (defaultFullPath != "")
            {
                string[][] localQueue = queue;

                if (!releasePrevious && releasedLine != 0) {
                    localQueue = new string[queue.Length][];

                    for (uint i = releasedLine; i < queue.Length; i++)
                    {
                        localQueue[i-releasedLine] = queue[i];

                    }
                }

                foreach (string[] logQ in localQueue)
                {
                    string path = "";
                    if (logQ[6] == "")
                    {
                        path = defaultFullPath;
                    }
                    else
                    {
                        path = logQ[6];

                    }

                    if (logQ[7] == "False")
                    {
                        Log(logQ[0] + "\t" + logQ[1] + "\t" + logQ[2] + "\t" + logQ[3], path, true);
                    }
                    else
                    {
                        Log(logQ[3], path, true);

                    }

                }

                if (resetQueue)
                {
                    ResetQueue();
                    releasedLine = 0;
                }
                else
                {
                    releasedLine = (uint)queue.Length - 1;

                }
            }
            else {
                MessageBox.Show("ReleaseQueue requires a default path defined! No logs have been saved!");

            }

            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                Monitor.Exit(locker);//unlock this function to allow the next thread to lock this function
            }

        }

        /// <summary>
        /// Queue a string of text to send for logging and debugging with the caller, line number and time appended at the begining - if not disabled.
        /// Use releaseQueue() to send the logs to the target file and reset the queue.
        /// </summary>
        /// <param name="logToSend">The string of text to queue for sending to the target file.</param>
        /// <param name="filesFullPath">The file and its full path to target when the queue is realeased.</param>
        /// <param name="tabRatio">The ratio of characters per tab, notepad.exe is 8 while most others are 4.</param>
        /// <param name="disableExtra">Weather to not append the date the log was queued, the line number and the caller that this was called from.</param>
        /// <param name="lineNumber"></param>
        /// <param name="caller"></param>
        public void QueueLog(string logToSend, string filesFullPath = "", int tabRatio = 4, bool disableExtra = false
            , [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null) {

            Array.Resize(ref queue, queue.Length + 1);
            int thisQueue = queue.Length-1;
            queue[thisQueue] = new string[8];//create a new queue
            int pLength = 0;
            int li = -1;
            int existingTabCount = 0;

            //fill the new queues information
            queue[thisQueue][0] = DateTime.Now.ToString();
            queue[thisQueue][1] = caller + "()";
            queue[thisQueue][2] = "@L" + lineNumber.ToString() + ":";
            queue[thisQueue][3] = logToSend;
            queue[thisQueue][6] = filesFullPath;
            queue[thisQueue][7] = disableExtra.ToString();

            if (!disableExtra && queue.Length > 1)
            {
                for (int i = 0; i < queue.Length; i++)
                {//loop until reach this queued message (the last queued message)
                    string tmpa = queue[i][1].Replace("\t", "");
                    string tmpb = queue[i][2].Replace("\t", "");

                    int length = queue[i][0].Length + tmpa.Length + tmpb.Length;

                    if (length > pLength)
                    {
                        li = i;//get the queue with the largest caller name and\or lineNumber
                    }

                    pLength = queue[i][0].Length + tmpa.Length + tmpb.Length;
                }

                foreach (string[] item in queue)
                {//this is probably causing issues
                    existingTabCount = queue[li][1].Split('\t').Length - 1;
                    float CallerFloatLength = (float)(queue[li][1].Replace("\t", "").Length / tabRatio) + existingTabCount;//get the tab length of the caller of the largest queue

                    existingTabCount = queue[li][2].Split('\t').Length - 1;
                    float LineFloatLength   = (float)(queue[li][2].Replace("\t", "").Length / tabRatio) + existingTabCount;//get the tab length of the @L of the largest queue

                    existingTabCount = queue[thisQueue][1].Split('\t').Length - 1;
                    queue[thisQueue][4] = ((float)(queue[thisQueue][1].Replace("\t", "").Length / tabRatio) + existingTabCount).ToString();//store the result so that it can be compared

                    existingTabCount = queue[thisQueue][2].Split('\t').Length - 1;
                    queue[thisQueue][5] = ((float)(queue[thisQueue][2].Replace("\t", "").Length / tabRatio) + existingTabCount).ToString();

                    int TabLength = (int)Math.Floor(CallerFloatLength);//get the tab length of the largest queues' caller text as litteral
                    int OtherTabLength = (int)Math.Floor(decimal.Parse(item[4]));//get the tab length of the other caller text as litteral

                    if (TabLength > OtherTabLength)
                    {//if the caller name is longer than the other caller name by at least 1 tab
                     //append extra tabs to this items' caller text (after)
                        int diff = TabLength - OtherTabLength;
                        for (int i = 0; i < diff; i++)
                        {
                            item[1] = item[1] + "\t";

                        }

                    }
                    else if (TabLength < OtherTabLength)
                    {
                        //append extra tabs to the largest queue caller text (after)
                        int diff = OtherTabLength - TabLength;
                        for (int i = 0; i < diff; i++)
                        {
                            queue[li][1] = queue[li][1] + "\t";

                        }

                    }

                    //handling tabbing for @L(lineNumber)
                    TabLength = (int)Math.Floor(LineFloatLength);//get the tab length of the largest queues' @L text
                    int CharLength = (int)((float)((LineFloatLength - TabLength)) * 4);//get how many characters into the next tab the text is in, 0.75=3, 0.5=2, 0.25=1

                    existingTabCount = item[2].Split('\t').Length-1;
                    OtherTabLength = (int)Math.Floor(decimal.Parse(item[5])) + existingTabCount;//get the tab length of the other @L text
                    int OtherCharLength = (int)((float)((decimal.Parse(item[5]) - (OtherTabLength - existingTabCount))) * 4);//0.75=3, 0.5=2, 0.25=1

                    if (TabLength > OtherTabLength)
                    {//if the @L full text is longer than the other caller full text by at least 1 tab
                     //append extra tabs to the OTHER @L full text (after)
                        int diff = TabLength - OtherTabLength;
                        for (int i = 0; i < diff; i++)
                        {
                            item[2] = item[2] + "\t";

                        }

                    }

                    existingTabCount = item[1].Split('\t').Length - 1;
                    item[4] = ((float)(item[1].Replace("\t", "").Length / tabRatio) + existingTabCount).ToString();//update tab count to account for the newly added tabbings

                    existingTabCount = item[2].Split('\t').Length - 1;
                    item[5] = ((float)(item[2].Replace("\t", "").Length / tabRatio) + existingTabCount).ToString();
                }
            }
            else {
                queue[thisQueue][4] = ((float)queue[thisQueue][1].Replace("\t", "").Length / tabRatio).ToString();//store the tab count
                queue[thisQueue][5] = ((float)queue[thisQueue][2].Replace("\t", "").Length / tabRatio).ToString();
            }

        }

        /// <summary>
        /// Immedietly sends a string of data to a file for logging and debugging with the caller,
        /// line number and time appended at the begining - if not disabled. Use on a single thread only, use QueueLog() and ReleaseQueue() instead for multi-threaded messages.
        /// </summary>
        /// <param name="logToSend">The text to pipe to the file</param>
        /// <param name="filesFullPath">The file the text will be piped to</param>
        /// <param name="disableExtra">Option to disable extra information (line number and caller)</param>
        /// <param name="lineNumber">The line this function was called from (the caller), default is to auto-get this result</param>
        /// <param name="caller">The function which contains this function caller, default is to auto-get this result</param>
		public void Log(string logToSend, string filesFullPath, bool disableExtra = false, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {
            fileI = new FileInfo(filesFullPath);
            DateTime now = DateTime.Now;

            if (!logToSend.Equals(""))
            {
                try
                {

                    Debug.WriteLine("");

                    //Debug.WriteLine("tring to send the message \"" + logToSend + "\" ");

                    if (File.Exists(filesFullPath))
                    {
                        if (!IsFileLocked(fileI))
                        {
                            TextWriter tsw = new StreamWriter(filesFullPath, true);
                            if (!disableExtra)
                            {
                                tsw.WriteLine(now + "\t" + caller + "()\t@L" + lineNumber + ":\t" + logToSend);

                            }
                            else {
                                tsw.WriteLine(logToSend);
                            }
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
                                    TextWriter tsw = new StreamWriter(filesFullPath, true);
                                    if (!disableExtra)
                                    {
                                        tsw.WriteLine(now + "\t" + caller + "()\t@L" + lineNumber + ":\t" + logToSend);
                                    }
                                    else
                                    {
                                        tsw.WriteLine(logToSend);
                                    }

                                    tsw.Close();
                                    tsw = null;
                                    Debug.WriteLine("retry completed, log sent.");
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
                    Log(now + "\t" + caller + "()\t@L" + lineNumber + ":\t" + logToSend, filesFullPath, true);

                }

            }
            else
            {

                try
                {

                    Debug.WriteLine("");

                    if (File.Exists(filesFullPath))
                    {
                        if (!IsFileLocked(fileI))
                        {
                            TextWriter tsw = new StreamWriter(filesFullPath, true);
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
                                    TextWriter tsw = new StreamWriter(filesFullPath, true);
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
                    Log(logToSend, filesFullPath);

                }

            }

        }

    }
}
