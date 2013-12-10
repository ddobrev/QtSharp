namespace QtCore
{
    public struct QList
    {
        public QListData d;

        #region WRITE

        // Plan:
        // 1. Wrap QtPrivate::RefCount - it's used as a member of QListData::Data;
        //    - QtPrivate::RefCount is not exported so it cannot be used; so the check below 'd->ref.isShared()' has to be replaced with the underlying logic;
        //      this logic can be located in the Qt docs for QList - "Internally, QList<T> is represented as an array of pointers to items of type T. 
        //      If T is itself a pointer type or a basic type that is no larger than a pointer, or if T is one of Qt's shared classes,
        //      then QList<T> stores the items directly in the pointer array." where 'shared classes' is a link to another article listing the shared classes      
        // 2. Mirror the QList::Node struct;
        // 3. Wrap, or copy the logic of, QTypeInfo<T>;
        // 4. Reimplement node_copy;
        // 5. Reimplement node_destruct;
        // 6. Reimplement node_construct;
        // 7. There's a call to QtPrivate::RefCount::deref() - how to work around that?
        // 8. Reimplement append.

        //        template <typename T>
//Q_OUTOFLINE_TEMPLATE void QList<T>::append(const T &t)
//{
//    if (d->ref.isShared()) {
//        Node *n = detach_helper_grow(INT_MAX, 1);
//        QT_TRY {
//            node_construct(n, t);
//        } QT_CATCH(...) {
//            --d->end;
//            QT_RETHROW;
//        }
//    } else {
//        if (QTypeInfo<T>::isLarge || QTypeInfo<T>::isStatic) {
//            Node *n = reinterpret_cast<Node *>(p.append());
//            QT_TRY {
//                node_construct(n, t);
//            } QT_CATCH(...) {
//                --d->end;
//                QT_RETHROW;
//            }
//        } else {
//            Node *n, copy;
//            node_construct(&copy, t); // t might be a reference to an object in the array
//            QT_TRY {
//                n = reinterpret_cast<Node *>(p.append());;
//            } QT_CATCH(...) {
//                node_destruct(&copy);
//                QT_RETHROW;
//            }
//            *n = copy;
//        }
//    }
//}

//        template <typename T>
//Q_OUTOFLINE_TEMPLATE typename QList<T>::Node *QList<T>::detach_helper_grow(int i, int c)
//{
//    Node *n = reinterpret_cast<Node *>(p.begin());
//    QListData::Data *x = p.detach_grow(&i, c);
//    QT_TRY {
//        node_copy(reinterpret_cast<Node *>(p.begin()),
//                  reinterpret_cast<Node *>(p.begin() + i), n);
//    } QT_CATCH(...) {
//        p.dispose();
//        d = x;
//        QT_RETHROW;
//    }
//    QT_TRY {
//        node_copy(reinterpret_cast<Node *>(p.begin() + i + c),
//                  reinterpret_cast<Node *>(p.end()), n + i);
//    } QT_CATCH(...) {
//        node_destruct(reinterpret_cast<Node *>(p.begin()),
//                      reinterpret_cast<Node *>(p.begin() + i));
//        p.dispose();
//        d = x;
//        QT_RETHROW;
//    }

//    if (!x->ref.deref())
//        dealloc(x);

//    return reinterpret_cast<Node *>(p.begin() + i);
//}

//        template <typename T>
//Q_INLINE_TEMPLATE void QList<T>::node_construct(Node *n, const T &t)
//{
//    if (QTypeInfo<T>::isLarge || QTypeInfo<T>::isStatic) n->v = new T(t);
//    else if (QTypeInfo<T>::isComplex) new (n) T(t);
//#if (defined(__GNUC__) || defined(__INTEL_COMPILER) || defined(__IBMCPP__)) && !defined(__OPTIMIZE__)
//    // This violates pointer aliasing rules, but it is known to be safe (and silent)
//    // in unoptimized GCC builds (-fno-strict-aliasing). The other compilers which
//    // set the same define are assumed to be safe.
//    else *reinterpret_cast<T*>(n) = t;
//#else
//    // This is always safe, but penaltizes unoptimized builds a lot.
//    else ::memcpy(n, static_cast<const void *>(&t), sizeof(T));
//#endif
//}

        #endregion
    }
}
