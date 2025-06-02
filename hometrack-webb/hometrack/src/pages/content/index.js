import classNames from "classnames/bind";
import style from "./index.module.scss";
import { listReport } from "./listcontent";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSortDown } from "@fortawesome/free-solid-svg-icons";

const cx = classNames.bind(style);

function Content() {
  const listR = listReport;
  return (
    <div className={cx("wrapper")}>
      <div className={cx("header")}>
        <div className={cx("header-content")}>Danh sách báo cáo</div>
        <div className={cx("header-all")}>
          <div className={cx('content-ct')}>Tất cả báo cáo</div>
          <FontAwesomeIcon className={cx('header-icon')} icon={faSortDown} />
          </div>
      </div>
      <div className={cx("main")}>
        {listR.map((item, index) => {
          return (
            <div className={cx("list-all")} key={index}>
              <div className={cx("content")}>
               <div className={cx('list-i-c')}>
                    <div className={cx('box-img')}>
                        <img
                          className={cx("list-img")}
                          src={item.img}
                          alt="ảnh báo cáo"
                        />
                    </div>
                    <div className={cx("list-content")}>
                      <div className={cx("list-content-all")}>
                        <div className={cx("list-content-rp")}>
                          Báo cáo #{item.id}
                        </div>
                        <div
                          className={cx("list-content-warning")}
                          style={{
                            backgroundColor: item.backGroud,
                            color: item.color,
                          }}
                        >
                          {item.report}
                        </div>
                      </div>
                      <div className={cx("list-content-ct")}>{item.content}</div>
                      <div className={cx("list-content-user")}>
                        <div className={cx("list-content-user-U")}>
                          Báo cáo bởi: {item.reportBy}
                        </div>
                        <div className={cx("list-content-user-y")}>{item.date}</div>
                      </div>
                    </div>
               </div>
              <div className={cx("btn")}>
                <div className={cx("btn-next")}>Bỏ qua</div>
                <div className={cx("btn-browse")}>Duyệt</div>
                <div className={cx("btn-clear")}>Xóa</div>
              </div>
            </div>
              </div>
          );
        })}
      </div>
      <div className={cx('footer')}>
        <div className={cx('footer-box-all')}>
        <div className={cx('footer-box')}>&larr;</div>
        <div className={cx('footer-box')}>1</div>
        <div className={cx('footer-box')}>2</div>
        <div className={cx('footer-box')}>&rarr;</div>

        </div>
      </div>
    </div>
  );
}

export default Content;
