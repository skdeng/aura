namespace Aura.VecMath
{
    class Mat4
    {
        readonly double[][] Data;

        public Mat4(double[] input)
        {
            Data = new double[4][];
            for (int i = 0; i < 4; i++)
            {
                Data[i] = new double[4];
                for (int j = 0; j < 4; j++)
                {
                    Data[i][j] = input[i * 4 + j];
                }
            }
        }

        public Mat4(string input)
        {
            string[] rows = input.Split(';');
            Data = new double[4][];
            for (int i = 0; i < 4; i++)
            {
                Data[i] = new double[4];
                string[] rowData = rows[i].Split(' ');
                for (int j = 0; j < 4; j++)
                {
                    Data[i][j] = double.Parse(rowData[j]);
                }
            }
        }

        public Vec3 TransformPoint(Vec3 point)
        {
            var returnVec = new Vec3();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    returnVec[i] += point[i] * Data[i][j];
                }
                returnVec[i] += Data[i][3];
            }
            return returnVec;
        }

        public Vec3 TransformVector(Vec3 point)
        {
            var returnVec = new Vec3();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    returnVec[i] += point[i] * Data[i][j];
                }
            }
            return returnVec;
        }
    }
}
