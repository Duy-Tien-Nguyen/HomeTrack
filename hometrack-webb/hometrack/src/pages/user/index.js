import classNames from "classnames/bind";
import style from "./index.module.scss";
import { listUsers } from "./listUser";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faGear, faLock, faRectangleXmark, faUser } from "@fortawesome/free-solid-svg-icons";

const cx = classNames.bind(style);

// const listU = listUsers

// function User() {
//     return ( <div>user</div> );
// }

// export default User;

const UserTable = () => {
  return (
    <div className={cx("wrapper")}>
      <div className={cx("header")}>
        <div className={cx("row", "headerRow")}>
          <div className={cx("cell")}>Tên người dùng</div>
          <div className={cx("cell")}>Email</div>
          <div className={cx("cell")}>Trạng thái</div>
          <div className={cx("cell")}>Gói dịch vụ</div>
          <div className={cx("cell")}>Thao tác</div>
        </div>

        {listUsers.map((item, idx) => (
          <div key={idx} className={cx("row")}>
            <div className={cx("cell", "cell-name")}>
              <div className={cx("item-name")}>{item.itemName}</div>
              <div className={cx("name")}>{item.name}</div>
            </div>
            <div className={cx("cell")}>{item.email}</div>
            <div className={cx("cell")}>
              <div
                className={cx("status")}
                style={{
                  color: item.colorStatus,
                  backgroundColor: item.colorbacgroudStatus,
                }}
              >
                {item.status}
              </div>
            </div>
            <div className={cx("cell")}>
              <span
                className={cx("service")}
                style={{
                  color: item.colorService,
                  backgroundColor: item.colorbacgroudService,
                }}
              >
                {item.service}
              </span>
            </div>
            <div className={cx("cell")}>
              <div className={cx("actions")}>
                <button className={cx("iconButton")}>
                  <FontAwesomeIcon
                    className={cx("xmark",'icon')}
                    icon={faRectangleXmark}
                  />
                </button>
                <button className={cx("iconButton")}>
                <FontAwesomeIcon
                    className={cx("gear",'icon')}
                    icon={faGear}
                  />
                </button>
                <button className={cx("iconButton")}>
                <FontAwesomeIcon
                    className={cx("user",'icon')}
                    icon={faUser}
                  />
                </button>
                <button className={cx("iconButton")}>
                <FontAwesomeIcon
                    className={cx("lock",'icon')}
                    icon={faLock}
                  />
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>

      <div className={cx("footer")}>
        <div className={cx("footer-box")}>
          <div className={cx("box")}>&larr;</div>
          <div className={cx("box")}>1</div>
          <div className={cx("box")}>2</div>
          <div className={cx("box")}>3</div>
          <div className={cx("box")}>&rarr;</div>
        </div>
      </div>
    </div>
  );
};

export default UserTable;
