import classNames from "classnames/bind";
import style from "./index.module.scss";
import { useNavigate } from "react-router-dom";

const cx = classNames.bind(style);

function Login() {
  const navigate = useNavigate();
  const handleLogin = () => {
   
    navigate("/"); 
  };
  return (
    <div className={cx("wrapper")}>
      <div className={cx("font-login")}>
        <div className={cx("header")}>Đăng nhập quản trị</div>
        <div className={cx("content")}>Admin Login</div>
        <div className={cx("login-form")}>
          <div className={cx("input-content-1")}>
            <div className={cx("name")}>Email</div>
            <input
              className={cx("input-name")}
              placeholder="Nhập email của bạn "
              name="user-name"
              required
            />
          </div>
          <div className={cx("input-content-2")}>
            <div className={cx("name")}>Nhận OTP</div>
            <div className={cx('input-name','input-otp')}>
              <input
                className={cx("input")}
                placeholder=" Nhận OTP "
                name="user-name"
                required
              />
              <button className={cx("button")}>Gửi</button>
            </div>

          </div>
          <button className={cx("submit")} onClick={handleLogin}>Đăng nhập</button>
        </div>
      </div>
    </div>
  );
}

export default Login;
