using System;
using System.Drawing;
using System.Windows.Forms;

public class CubeForm : Form
{
    private Timer timer = new Timer();
    private float A = 0, B = 0, C = 0; // 회전 각도
    private float cubeWidth = 50;
    private int distanceFromCam = 150;
    private float K1 = 50;

    private bool isDragging = false; // 드래그 상태
    private Point lastMousePosition; // 마지막 마우스 위치

    public CubeForm()
    {
        this.DoubleBuffered = true; // 화면 깜박임 방지
        this.Width = 800;
        this.Height = 600;

        // 타이머 설정
        timer.Interval = 16; // 약 60FPS
        timer.Tick += (s, e) => { C += 0.01f; this.Invalidate(); };
        timer.Start();

        // 마우스 이벤트 등록
        this.MouseDown += CubeForm_MouseDown;
        this.MouseMove += CubeForm_MouseMove;
        this.MouseUp += CubeForm_MouseUp;
    }

    // 마우스 클릭
    private void CubeForm_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            isDragging = true;
            lastMousePosition = e.Location;
        }
    }

    // 마우스 이동
    private void CubeForm_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDragging)
        {
            // 마우스 이동량 계산
            float deltaX = e.X - lastMousePosition.X;
            float deltaY = e.Y - lastMousePosition.Y;

            // 이동량을 회전 각도에 반영
            A += deltaY * 0.01f; // 수직 이동은 X축 회전
            B += deltaX * 0.01f; // 수평 이동은 Y축 회전

            lastMousePosition = e.Location; // 마지막 위치 갱신
            this.Invalidate(); // 화면 갱신 요청
        }
    }

    // 마우스 버튼 해제
    private void CubeForm_MouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            isDragging = false;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(Color.Black);

        // 큐브 그리기
        for (float cubeX = -cubeWidth; cubeX <= cubeWidth; cubeX += 5)
        {
            for (float cubeY = -cubeWidth; cubeY <= cubeWidth; cubeY += 5)
            {
                DrawSurface(g, cubeX, cubeY, -cubeWidth, Color.Red);
                DrawSurface(g, cubeWidth, cubeY, cubeX, Color.Green);
                DrawSurface(g, -cubeWidth, cubeY, -cubeX, Color.Blue);
                DrawSurface(g, -cubeX, cubeY, cubeWidth, Color.Yellow);
                DrawSurface(g, cubeX, -cubeWidth, -cubeY, Color.Cyan);
                DrawSurface(g, cubeX, cubeWidth, cubeY, Color.Magenta);
            }
        }
    }

    private void DrawSurface(Graphics g, float cubeX, float cubeY, float cubeZ, Color color)
    {
        float x = calculateX(cubeX, cubeY, cubeZ);
        float y = calculateY(cubeX, cubeY, cubeZ);
        float z = calculateZ(cubeX, cubeY, cubeZ) + distanceFromCam;

        float ooz = 1 / z;
        int xp = (int)(this.Width / 2 + K1 * ooz * x);
        int yp = (int)(this.Height / 2 + K1 * ooz * y);

        using (Brush brush = new SolidBrush(color))
        {
            g.FillRectangle(brush, xp, yp, 4, 4); // 점을 작은 사각형으로 표시
        }
    }

    private float calculateX(float i, float j, float k) =>
        (float)(j * Math.Sin(A) * Math.Sin(B) * Math.Cos(C) -
                k * Math.Cos(A) * Math.Sin(B) * Math.Cos(C) +
                j * Math.Cos(A) * Math.Sin(C) +
                k * Math.Sin(A) * Math.Sin(C) +
                i * Math.Cos(B) * Math.Cos(C));

    private float calculateY(float i, float j, float k) =>
        (float)(j * Math.Cos(A) * Math.Cos(C) +
                k * Math.Sin(A) * Math.Cos(C) -
                j * Math.Sin(A) * Math.Sin(B) * Math.Sin(C) +
                k * Math.Cos(A) * Math.Sin(B) * Math.Sin(C) -
                i * Math.Cos(B) * Math.Sin(C));

    private float calculateZ(float i, float j, float k) =>
        (float)(k * Math.Cos(A) * Math.Cos(B) -
                j * Math.Sin(A) * Math.Cos(B) +
                i * Math.Sin(B));

    [STAThread]
    static void Main()
    {
        Application.Run(new CubeForm());
    }
}
