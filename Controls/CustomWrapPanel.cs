using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Foundation;

namespace FSGaryityTool_Win11.Controls
{
    public class CustomWrapPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            Size totalSize = new Size(); // �ܵĳߴ�
            double lineHeight = 0; // ��ǰ�еĸ߶�

            foreach (UIElement child in Children) // ����������Ԫ��
            {
                child.Measure(availableSize); // ������Ԫ�صĳߴ�
                FrameworkElement feChild = child as FrameworkElement; // �� child ת��Ϊ FrameworkElement
                double childWidth = double.IsNaN(feChild.Width) ? feChild.MinWidth : feChild.Width; // �����Ԫ���й̶��Ŀ�ȣ���ʹ�� Width������ʹ�� MinWidth

                if (totalSize.Width + childWidth > availableSize.Width) // �����Ԫ�صĿ�ȳ����˿��ÿ�ȣ�����
                {
                    totalSize.Width = 0; // �����ܿ��Ϊ0
                    totalSize.Height += lineHeight; // �����ܸ߶�
                }

                lineHeight = Math.Max(lineHeight, feChild.Height); // �и�Ϊ��ǰ��������Ԫ�������ĸ߶�
                totalSize.Width += childWidth; // �����ܿ��
            }


            totalSize.Height += lineHeight; // ������һ�еĸ߶�

            return totalSize; // �����ܵĳߴ�
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect finalRect = new Rect(); // ���յĲ��־���
            double lineHeight = 0; // ��ǰ�еĸ߶�
            UIElement lastChild = null; // ��һ����Ԫ��

            double totalWidth = 0; // �����ۼӵ�ǰ�е��ܿ��

            foreach (UIElement child in Children) // ����������Ԫ��
            {
                FrameworkElement feChild = child as FrameworkElement; // �� child ת��Ϊ FrameworkElement
                double childWidth = double.IsNaN(feChild.Width) ? feChild.MinWidth : feChild.Width; // �����Ԫ���й̶��Ŀ�ȣ���ʹ�� Width������ʹ�� MinWidth

                if (finalRect.Left + childWidth > finalSize.Width) // �����Ԫ�صĿ�ȳ����˿��ÿ�ȣ�����
                {
                    // �������һ�е����һ����Ԫ�صĿ�������ʣ��ռ�
                    FrameworkElement feLast = lastChild as FrameworkElement; // �� lastChild ת��Ϊ FrameworkElement
                    if (feLast != null && totalWidth != lastChild.DesiredSize.Width)
                    {
                        double minWidth = feLast.MinWidth; // ��ȡ��С���

                        // ���㵱ǰ�г�ȥ���һ��Ԫ��֮ǰ��Ԫ�ص��ܿ��
                        double previousWidth = totalWidth - lastChild.DesiredSize.Width;

                        // �����ǰ�е����һ��Ԫ��û�й̶��Ŀ�ȣ��Ͳ���Ҫ�������Ŀ��
                        if (double.IsNaN(feLast.Width))
                        {
                            // ������һ����Ԫ�صĿ�������ʣ��ռ�
                            Rect lastChildRect = new Rect(finalRect.X - minWidth, finalRect.Y, finalSize.Width - previousWidth, lineHeight);
                            lastChild.Arrange(lastChildRect);
                        }
                    }

                    finalRect.Y += lineHeight; // ����Y���굽��һ��
                    finalRect.X = 0; // X��������Ϊ0
                    finalRect.Height = child.DesiredSize.Height; // ���¾��εĸ߶�Ϊ��ǰ��Ԫ�صĸ߶�
                    lineHeight = child.DesiredSize.Height; // �����и�Ϊ��ǰ��Ԫ�صĸ߶�

                    totalWidth = 0; // ���õ�ǰ�е��ܿ��
                }

                lineHeight = Math.Max(lineHeight, child.DesiredSize.Height); // �и�Ϊ��ǰ��������Ԫ�������ĸ߶�
                finalRect.Width = childWidth; // ���εĿ��Ϊ��ǰ��Ԫ�صĿ��
                finalRect.Height = child.DesiredSize.Height; // ���εĸ߶�Ϊ��ǰ��Ԫ�صĸ߶�

                child.Arrange(finalRect); // ������Ԫ���ھ�����
                finalRect.X += finalRect.Width; // ����X���굽��һ����Ԫ�ص�λ��

                totalWidth += childWidth; // �ۼӵ�ǰ�е��ܿ��

                lastChild = child; // ������һ����Ԫ��Ϊ��ǰ��Ԫ��
            }

            // �������һ�е����һ����Ԫ�صĿ�������ʣ��ռ�
            FrameworkElement fd = lastChild as FrameworkElement; // �� lastChild ת��Ϊ FrameworkElement
            if (fd != null && totalWidth == lastChild.DesiredSize.Width)
            {
                double minWidth = fd.MinWidth; // ��ȡ��С���

                // ���㵱ǰ�г�ȥ���һ��Ԫ��֮ǰ��Ԫ�ص��ܿ��
                double previousWidth = totalWidth - lastChild.DesiredSize.Width;

                // �����ǰ�е����һ��Ԫ��û�й̶��Ŀ�ȣ��Ͳ���Ҫ�������Ŀ��
                if (double.IsNaN(fd.Width))
                {
                    // ������һ����Ԫ�صĿ�������ʣ��ռ�
                    Rect lastChildRect = new Rect(finalRect.X - minWidth, finalRect.Y, finalSize.Width - previousWidth, lineHeight);
                    lastChild.Arrange(lastChildRect);
                }
            }

            return finalSize; // �������յĳߴ�
        }

    }
}
