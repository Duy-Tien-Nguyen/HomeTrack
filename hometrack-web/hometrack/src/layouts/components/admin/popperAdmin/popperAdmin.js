import classNames from "classnames/bind";
import style from "./popperAdmin.module.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faRightFromBracket } from "@fortawesome/free-solid-svg-icons";
import { Link } from "react-router-dom";


const cx = classNames.bind(style);


function PopperAdmin() {
    return ( <div className={cx('wrapper')}>
        <Link className={cx('link-login')} to='/login'>
            
       <div className={cx('item')}>
        <div className={cx('icon')}>
            <FontAwesomeIcon className={cx('icon-c')} icon={faRightFromBracket}  />
        </div> 
        <div className={cx('content')}>Đăng xuất</div>
        </div>
        </Link>
    </div> );
}

export default PopperAdmin;

