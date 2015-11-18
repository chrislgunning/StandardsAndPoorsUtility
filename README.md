# StandardsAndPoorsUtility
This is a simple web utility for retrieving S &amp; P 500 historical data by a date range.

This utility is implemented to be run under Linux (more specifically Ubuntu) with the "mono" package. 
If you prefer Windows, drop the code into a Visual Studio project and be on your merry way.


1.) Ensure that "mono" is installed in your Ubuntu environment:

Sanity check in terminal: 'sudo apt-get install mono-mcs'

2.) Compile the code: in terminal: 'mcs StandardsAndPoorsUtility.cs'

3.) Run the code in terminal: 

'mono StandardsAndPoorsUtility.exe {File Name} {Start Month (Zero Based)} {Start Day} {Start Year} {End Month (Zero Based)} {End Day} {End Year}'

Example:

Output File: s_and_p_500_data.csv

Start Date: April 4, 2012

End Date: May 14, 2015


Command in terminal to retrieve data:

'mono StandardsAndPoorsUtility.exe s_and_p_500_data.csv 3 4 2012 4 14 2015'






