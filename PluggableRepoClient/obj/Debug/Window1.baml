   M S B A M L   `   `   `  ����N  JPluggableRepoClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:PluggableRepoClientPluggableRepoClient  5   	   !    PluggableRepoClient.Window1   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   3local!clr-namespace:PluggableRepoClient     5   	     � Title$  Code Window��5   	   $	��500��6   $	��400��6*   .��5      � $��10q�6	    � RowDefinitions 5      �� 5      $��25 �6   6$   �� 5       $��2* �6   6$   �� 5!      $��25 �6   6$   �� 5"      $
��1.2* �6   6&   �� 5#      $��40 �6   6$   .+�5%      �� -   5        $��	codeLabel��5%      $��0��6    �� TextAlignment$ Center=�6,   $��3q�6C   $��0q�6O   .��6c   	Code View6n   ,� 5&      $��1��6   $��1q�6,   
���   � ,� IsToolBarVisible .  " ,� VerticalScrollBarVisibility$
 Auto=�6m   .��5'      /� -   5         �Name$ codeView��5'       /� 
FontFamily$ Consolas)�6%    /� FontSize$ 12(�6;   .T�5(   
   J� -   5        $
 main��5(      .*�6    .*�5*      this is test�� with second line5,      5-   	   5.      �� 5/      -   5        $��
deps2Label��5/      $��2��6   $ Center=�6-   $��3q�6D   $��0q�6P   .��6d   
MetaData6n   ,� 50      $��3��6   $��1q�6,   
���   � .  $
 Auto=�6m   .��51      /� -   5        $ metaDataView��51      $ Consolas)�6)   $ 12(�6?   .T�52   
   J� -   5         J�Name$ metaDataText��52      .*�6*   .*�54      this is test�� with second line56      57   	   58      �� 59      $��4��6   	 �� Orientation$	 
Horizontal=�6   $��10q�67   $��Center=�6C   .+�5:      �� -   5        $��
exitButton��5:      $��60��6!   .��6E   Exit6K   5;      5<      5=      