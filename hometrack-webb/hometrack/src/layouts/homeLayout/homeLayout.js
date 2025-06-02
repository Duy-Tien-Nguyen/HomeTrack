import classNames from "classnames/bind";
import style from "./homeLayout.module.scss";
import Sidebar from "../components/sidebar/sidebar";
import Search from "../components/search/search";
import Information from "../components/information/information";

const cx = classNames.bind(style);

function HomeLayout({ children }) {
  return (
    <div className={cx("wrapper")}>
      <div className={cx("header")}>
        <div className={cx("header-left")}>Dashboard</div>
        <div className={cx("header-right")}>
          <div className={cx('content')}>
            <div className={cx('content-1')}>Tổng quan</div>
            <div className={cx('content-2')}>Chào mừng trở lại, Admin!</div>
          </div>
          <div className={cx("inform")}>
            <div className={cx("search")}>
              <Search />
            </div>
            <div className={cx("information")}>
              <Information />
            </div>
          </div>
        </div>
      </div>
      <div className={cx("main")}>
        <div className={cx("main-sidebar")}>
          <Sidebar />
        </div>
        <div className={cx("main-content")}>{children}</div>
      </div>
    </div>
  );
}

export default HomeLayout;
