namespace CRUDTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //arrange
            MyMath mm = new MyMath();
            int input1 = 10, input2 = 5;
            int expected = 15;

            //act
            int actual = mm.Add(input1, input2);

            //assert
            Assert.Equal(expected, actual);
        }
    }
}