   M S B A M L   `   `   `  ����@  <hotel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:hotelhotel  5          hotel.CustomerDetail   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      %localclr-namespace:hotel     5        N� Title$  CustomerDetail��5	      .��5      �� $
��Auto=�6   $��Disabled=�66   .��5   
   � $��1��6   $��20q�6   
������� � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $��* �6    .+�5      �� $��Chi tiết khách hàng��6   $��24(�63   $
��Bold"�6A   $��0,0,0,20q�6S   $��0��6e   �� 5      $��1��6   .+�5      �� $��Họ và tên: ��6   $
��Bold"�6+   �� 5      -   5        $��FullNameText��5      �� 5      $��Email: ��6   $
��Bold"�6'   $��0,10,0,0q�69   �� 5      -   5        $��	EmailText��5      �� 5      $��Số điện thoại: ��6   $
��Bold"�6/   $��0,10,0,0q�6A   �� 5       -   5        $��	PhoneText��5       �� 5"      $��Địa chỉ: ��6   $
��Bold"�6)   $��0,10,0,0q�6;   �� 5#      -   5        $��AddressText��5#      �� 5%      $��Sinh nhật: ��6   $
��Bold"�6+   $��0,10,0,0q�6=   �� 5&      -   5        $��DOBText��5&      �� 5(      $��CCCD: ��6   $
��Bold"�6&   $��0,10,0,0q�68   �� 5)      -   5        $��CCCDText��5)      �� 5+      $
��Bold"�6   $��Lịch sử thuê phòng��6*   $��18(�6D   $��	0,20,0,10q�6R   &   System.Windows.Controls.DataGrid  5,      -   5        $��ReservationDataGrid��5,         AutoGenerateColumns$
 True��62      
IsReadOnly$
 True��6M   5-      5.      5/      50      