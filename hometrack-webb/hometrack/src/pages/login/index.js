import classNames from "classnames/bind";
import style from "./index.module.scss";
import { useNavigate } from "react-router-dom";

const cx = classNames.bind(style);

function Login() {
  const navigate = useNavigate();

  // Hàm xử lý login (demo không kiểm tra email/mật khẩu)
  const handleLogin = () => {
    // Có thể kiểm tra dữ liệu ở đây
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
            <div className={cx("name")}>Mật khẩu</div>
            <input
              className={cx("input-name")}
              placeholder="nhập mật khẩu "
              name="user-name"
              required
            />
          </div>
          <button className={cx("submit")} onClick={handleLogin}>Đăng nhập</button>
        </div>
      </div>
    </div>
  );
}

export default Login;
