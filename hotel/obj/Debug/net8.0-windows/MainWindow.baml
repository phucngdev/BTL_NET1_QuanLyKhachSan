   M S B A M L   `   `   `  ����@  <hotel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:hotelhotel  5   	       hotel.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:hotel     5   	     � Title$  	Dashboard��5   	   $	��450��6   $	��800��6(    � 	Resources 5
         MenuButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType �� Padding �� Triggers P� Property	 P� Value��%�  &	        �� 5   
   " ��6(   .�5      ��  � ��
 ���� 6+   �� 5       � ��
 �����6+   �� 5       � ��$ 0q�60   �� 5       � ��$ 40��6'   �� 5       � ��$	 0,5q�6'   �� 5       �@�FocusVisualStyle 61   Z� �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   #��z��5   !   #��z��5   !   $
 20,0q�5   !   ._�5      �� $
��Left=�6/   $��Center=�6J   5       5      P� 5       �@J�IsMouseOver$
	 True��6=   . �5   "   ��  � �� �#1E32666?   5      P� 5       � ��$
	 True��6;   . �5    "   ��  � �� �#1523476?   5!      5#      5%      5&      5'      .��5)      � 
 � ColumnDefinitions
 5*   
   �� 5+      $	��200 �6   �� 5,      $��* �6   .+�5/   
   �� $��0��6   ���#2B4380.+�50      �� $��Quản lý khách sạn��6   
�������$��20(�6D   $��20,20,20,20q�6R   �� 51      $
��10,0q�6   .+�52      �� -   5         ��Name$ DashboardButton��52      #��[  62   $��Trang chủ��6[   6o   �� 53      -   5        $ RoomBookingButton��53      #��[  64   $��Đặt phòng��6]   6q   �� 54      -   5        $ 
RoomBooked��54      #��[  6-   $��Quản lý đặt phòng��6V   6r   �� 55      -   5        $ RoomManagementButton��55      #��[  67   $��Quản lý phòng��6`   6x   �� 56      -   5        $ ServicesButton��56      #��[  61   $��Quản lý dịch vụ��6Z   6t   �� 57      -   5        #��[  57      $��Danh sách khách hàng��6B   6a   �� 58      -   5        #��[  58      $��Quản lý nhân viên��6B   6^   59      5:       � 5=   
   -   5          �Name$ 	MainFrame��5=      $��1��6#     � NavigationUIVisibility$ Hidden=�63   5>      5?      